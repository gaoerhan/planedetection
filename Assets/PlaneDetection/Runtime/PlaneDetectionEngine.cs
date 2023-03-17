using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace detect
{
    public class PlaneDetectionEngine
    {

        private static volatile PlaneDetectionEngine instance;
        private static object syncRoot = new Object();
        public string className = "com.seedreality.seedplugin.SeedPlugin";
        public AndroidJavaObject pluginObject;

        private OnInitCallback mInitCallback;
        private PlaneDetectionEngine() { }

        public static PlaneDetectionEngine Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new PlaneDetectionEngine();
                    }
                }

                return instance;
            }
        }

        public void initEngine(OnInitCallback initCallback, string game)
        {
            mInitCallback = initCallback;
            pluginObject = new AndroidJavaClass(className).CallStatic<AndroidJavaObject>("GetInstance", game);
            pluginObject.Call("initEngine");
        }

        public void releaseEngine(string game)
        {
            pluginObject.Call("releaseEngine");
        }

        public float[] getData(float x, float y, float z, float qw, float qx, float qy, float qz)
        {
            return pluginObject.Call<float[]>("getData", x, y, z, qw, qx, qy, qz);
        }



    }



    public interface OnInitCallback
    {
        void SeedInitCallBack(string success);
    }
}