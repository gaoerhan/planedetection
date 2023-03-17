using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

// 有关概述，请参见_ReadMe.txt
[ExecuteInEditMode]
public class CommandBufferBlurRefraction : MonoBehaviour
{
	public Shader m_BlurShader;
	private Material m_Material;

	private Camera m_Cam;

	// 我们希望在任何一个渲染我们的摄像机上添加一个命令缓冲区，
	// 所以要有一个它们的 dictionary。
	private Dictionary<Camera,CommandBuffer> m_Cameras = new Dictionary<Camera,CommandBuffer>();

	// 从我们添加到的所有摄像机中删除命令缓冲区
	private void Cleanup()
	{
		foreach (var cam in m_Cameras)
		{
			if (cam.Key)
			{
				cam.Key.RemoveCommandBuffer (CameraEvent.AfterSkybox, cam.Value);
			}
		}
		m_Cameras.Clear();
		Object.DestroyImmediate (m_Material);
	}

	public void OnEnable()
	{
		Cleanup();
	}

	public void OnDisable()
	{
		Cleanup();
	}

	/// <summary>
	/// 每当任何相机将渲染我们，添加一个命令缓冲区来做这件事
	/// OnWillRenderObject : 如果对象可见，则为每个摄像机调用一次。
	/// </summary>
	public void OnWillRenderObject()
	{
		// 游戏对象和脚本都是激活状态
		var act = gameObject.activeInHierarchy && enabled;
		if (!act)
		{
			Cleanup();
			return;
		}
		
		// 当前渲染正在使用的 Camera，仅用于底层渲染控制
		var cam = Camera.current;
		if (!cam)
			return;

		CommandBuffer buf = null;
		// 我们已经在这个摄像机上添加了命令缓冲区了吗？那就没事做了。
		if (m_Cameras.ContainsKey(cam))
			return;
		 
		if (!m_Material)
		{
			// 使用指定的着色器创建材质
			m_Material = new Material(m_BlurShader);
			// HideFlags.HideAndDontSave :
			// 游戏对象不会显示在层次结构中，不会保存到场景中，也不会被Resources卸载。
			// 这通常用于由脚本创建并且完全由脚本控制的 GameObject。 
			m_Material.hideFlags = HideFlags.HideAndDontSave;
		}

		// 保存本 Camera 的命令缓冲区。
		buf = new CommandBuffer();
		buf.name = "Grab screen and blur";
		m_Cameras[cam] = buf;

		// 把屏幕拷贝到临时渲染纹理

		// Shader.PropertyToID    获取着色器属性名称的唯一标识符。
		// 使用属性标识符比将字符串传递到所有材质属性函数更有效。
		// 在 Unity 中，着色器属性的每个名称（例如 _MainTex 或 _Color）均分配有唯一 整数，
		// 在整个游戏中，该整数均保持相同。
		// 在游戏的不同次运行之间或在不同机器之间，这些数字不同，
		// 因此不要存储或通过网络发送这些数字。
		int screenCopyID = Shader.PropertyToID("_ScreenCopyTexture");
		// 添加“获取临时渲染纹理”命令。
		// nameID 	此纹理的着色器属性名称。
		// width 	像素宽度，或为 -1，表示“摄像机像素宽度”。
		// height 	像素高度，或为 -1，表示“摄像机像素高度”。
		// depthBuffer 	深度缓冲区位（0、16 或 24）。
		// filter 	纹理过滤模式（默认为 Point）。
		buf.GetTemporaryRT (screenCopyID, -1, -1, 0, FilterMode.Bilinear);
		// 添加“对渲染纹理执行 blit 操作”命令。
		// 源纹理或渲染目标将作为“_MainTex”属性传递给材质。
		buf.Blit (BuiltinRenderTextureType.CurrentActive, screenCopyID);
		int blurredID = Shader.PropertyToID("_Temp1");
		int blurredID2 = Shader.PropertyToID("_Temp2");
		// 获取两个更小的临时渲染纹理。
		// -2为“摄像机像素宽度”1/2 大小。
		buf.GetTemporaryRT (blurredID, -2, -2, 0, FilterMode.Bilinear);
		buf.GetTemporaryRT (blurredID2, -2, -2, 0, FilterMode.Bilinear);
		// 将屏幕副本降采样为较小的临时渲染纹理1
		buf.Blit (screenCopyID, blurredID);
		// 释放屏幕临时渲染纹理
		buf.ReleaseTemporaryRT (screenCopyID);

		// 添加“设置全局着色器向量属性”命令。
		// horizontal blur 水平模糊
		buf.SetGlobalVector("offsets", new Vector4(2.0f/Screen.width,0,0,0));
		buf.Blit (blurredID, blurredID2, m_Material);
		// vertical blur 垂直模糊
		buf.SetGlobalVector("offsets", new Vector4(0,2.0f/Screen.height,0,0));
		buf.Blit (blurredID2, blurredID, m_Material);
		// horizontal blur 水平模糊
		buf.SetGlobalVector("offsets", new Vector4(4.0f/Screen.width,0,0,0));
		buf.Blit (blurredID, blurredID2, m_Material);
		// vertical blur 垂直模糊
		buf.SetGlobalVector("offsets", new Vector4(0,4.0f/Screen.height,0,0));
		buf.Blit (blurredID2, blurredID, m_Material);

		// 添加“设置全局着色器纹理属性”命令（引用 RenderTexture）。
		buf.SetGlobalTexture("_GrabBlurTexture", blurredID);

		cam.AddCommandBuffer (CameraEvent.AfterSkybox, buf);
	}	
}
