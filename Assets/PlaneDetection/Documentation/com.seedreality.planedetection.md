#Seed Engine

Introduction: Currently, SDK supports the plane detection function, which can obtain ground height data relative to the head, and desktop height data relative to the head.

## PlaneDetection Component

1.Use [RequireComponent (typeof (PlaneDetection))] to rely on components

2.Use GetComponent<PlaneDetection>() to obtain the component , you can call the relevant interface through the component


### getPlaneData()

'float[] getPlaneData()'
get plane data 

**注解**
Obtaining planar detection data is time-consuming. It is recommended to start a sub thread 
The height of the plane is relative to the height of the helmet at the time, in meters, such as 1.5m from the ground to the helmet. Normally, the returned x should be around - 1.5


**返回**
Returns a planar coordinate data ,   the length of array is 3
float[0]:   Height of the ground relative to the head 
float[1]:	Height of the desk relative to the head
float[2]:	meaningless currently

The return value needs to be judged，if float[0]=-1 or float[1] = -1000 indicates failure



### testPlaneOn()
'void testPlaneOn(bool open)'
Whether to open the test plane，true indicates open，false indicates close, default close.  

**注解**
For test use, after obtaining plane data, display the preset plane, a translucent preset body with a plane of 2m x 2m