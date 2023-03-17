
PlaneDetection 脚本组件

1.可通过[RequireComponent(typeof(PlaneDetection))]依赖组件

2.获取组件 GetComponent<PlaneDetection>()后，可通过组件调用相关接口



### getPanelData()

'float[] getPanelData()'
平面检测数据获取

**注解**
获取平面检测数据，该方法比较耗时，建议开启子线程调用
平面的高度是在相对与当时头盔的高度，单位m，比如地面距离头盔1.5m， 正常情况下，返回的y应在 - 1.5 左右


**返回**
返回平面坐标数据数组,数组长度为3
float[0]:   地面相对于头部高度
float[1]:	桌面相对于头部高度
float[2]:	暂无含义

需要对返回值进行判断，当float[0]=-1 或 float[1] = -1000 表示失败




### testPlaneOn()
'void testPlaneOn(bool open)'
是否开启测试平面，true表示开启，false表示关闭。  

**注解**
测试使用，获取到平面数据后，显示预设的平面，平面为2m x 2m的半透明预设体
