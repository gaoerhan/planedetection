using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace detect
{
    public class PlaneDetection : MonoBehaviour, OnInitCallback
    {

        private bool isSeedInitSuccess;

        private float[] currentDatas;

        private GameObject PrefabFloor;

        private GameObject PlaneFloor;

        private GameObject PrefabDesk;

        private GameObject PlaneDesk;

        private bool IsOpenTestPlane;

        private bool IsDetected;


        // Start is called before the first frame update
        void Start()
        {
#if UNITY_ANDROID

            PlaneDetectionEngine.Instance.initEngine(this, gameObject.name);
            PrefabFloor = (GameObject)Resources.Load("Prefabs/GlassPlane");
            PrefabDesk = (GameObject)Resources.Load("Prefabs/GlassPlane");
            PlaneFloor = Instantiate(PrefabFloor);
            PlaneDesk = Instantiate(PrefabDesk);
            PlaneFloor.SetActive(false);
            PlaneDesk.SetActive(false);
            PlaneFloor.name = "FloorPlane";
            PlaneDesk.name = "DeskPlane";
#endif
        }
        // Update is called once per frame
        void Update()
        {
            if (isSeedInitSuccess && IsOpenTestPlane)
            {
                if (!IsDetected)
                {              
                    currentDatas = PlaneDetectionEngine.Instance.getData(1, 1, 1, 1, 1, 1, 1);

                    if (currentDatas[0] == -1 || currentDatas[1] == -1000) return;
                    else IsDetected = true;
                    Debug.Log($"testlog detect plane success");
                }

                //the height from floor to head
                if (currentDatas[0] != -1)
                {
                    PlaneFloor.SetActive(true);
                    PlaneFloor.transform.localPosition = new Vector3(0, 1.5f, 0);

                    //"_MainTex"是主要的漫反射纹理，也能通过 mainTextureScale 属性访问
                    //"_BumpMap"是法线贴图
                    //"_Cube"是反射cubemap.（盒子贴图）
                   // PlaneFloor.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(5, 5));

                }
                else
                {
                    PlaneFloor.SetActive(false);
                }

                //the height from desk to head
                if (currentDatas[1] != -1)
                {
                    PlaneDesk.SetActive(true);
                    PlaneDesk.transform.localPosition = new Vector3(0, -2f, 0);
                }
                else
                {
                    PlaneDesk.SetActive(false);
                }
            }

        }

        /// <summary>
        /// start plane prefab display
        /// </summary>
        /// <param name="open"> true 标识使用预设平面  false 表示关闭</param>
        public void testPlaneOn(bool open)
        {
            IsOpenTestPlane = open;
        }

  
        public float[] getPlaneData()
        {
            if (isSeedInitSuccess)
            {
                return PlaneDetectionEngine.Instance.getData(0, 1, 2, 3, 4, 5, 6);
            }
            else
            {
                return new float[] { -1, -1000, 0 };
            }
        }

        public void SeedInitCallBack(string success)
        {

            if ("true".Equals(success))
            {
                isSeedInitSuccess = true;
            }
            else
            {
                isSeedInitSuccess = false;
            }
        }

    }
}