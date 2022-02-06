using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Linq; //byte[]のconcatに使えます


namespace BodyOperations
{
    [RequireComponent(typeof(Animator))]
    public class BodyController : MonoBehaviour
    {
        private Animator animator;
        double rotate_global_deg;
        int frame_counter = 0;
        float xdir;
        float ydir;
        float zdir;
        private Transform target;
        Transform target_transform;
        private Vector3 _offset = Vector3.up;
        Vector3 RHPVECTOR;
        Vector3 shoulder_R_Vector;//  肩（要変換）
        Vector3 upperArm_R_Vector;//  上腕
        Vector3 elbow_R_Vector; //  肘
        Vector3 hand_R_Vector; //  手首
        Vector3 indexDistal_R_Vector; //  中指先端（要変換）
        Vector3 shoulder_L_Vector; //aaa
        Vector3 upperArm_L_Vector;
        Vector3 elbow_L_Vector;
        Vector3 hand_L_Vector;
        Vector3 indexDistal_L_Vector;
        Vector3 origin_position;

        Vector3 UpperBody_Vector;
        Vector3 Spine_Vector; // Controll Upper Body
        Vector3 Hip_Vector; // Controll All body. Base of Humanoid

        // UDP通信設定 (クライアント)
        const int port = 8890;
        string hostIP = "127.0.0.1";
        
        IPEndPoint local;
        IPEndPoint remote;
        UdpClient client;

        [System.Serializable]
        public class BodyTrackingData
        {
            public int attack;
            public int defense;
        }
        
        private void Start()
        {
            Debug.Log("Start");
            animator = GetComponent<Animator>();
            GameObject target = GameObject.Find("Sphere");
            target_transform = target.transform;
            Transform hip_T = animator.GetBoneTransform(HumanBodyBones.Hips);
            local = new IPEndPoint(IPAddress.Parse(hostIP), port);
            remote = new IPEndPoint(IPAddress.Parse(hostIP), port);
            client = new UdpClient(local);

        }

        private void Update()
        {
            // まずはUpperArmが水平であると仮定してelbowを
            // _animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).Rotate(new Vector3(0,0,0.1f));
            frame_counter = frame_counter +1;
            /*
            rotate_global_deg = 30*Math.Sin((double)frame_counter/10);
            xdir = (float)Math.Cos(rotate_global_deg * 3.14/180);
            ydir = 0;
            zdir = (float)Math.Sin(rotate_global_deg * 3.14/180);
            //Debug.Log(buffer + " , " + frame_counter + ", " + rotate_global_deg + " Look for " + xdir + "," + zdir);
            RHPVECTOR = new Vector3(xdir,ydir,zdir);
            
            // Calculate vector to target from selected bone.
            var dir = RHPVECTOR;
            // Rotate : 回転対象の軸が向くベクトルの設定
            var rawVector = Quaternion.LookRotation(dir);
            // compensate : 腕はY軸がBoneだったのでVector3.up（本来のY軸）をVector3.forward（Z軸）への回転に変換
            var offset = Quaternion.LookRotation(Vector3.up,Vector3.forward); // 1st argument vector to 2nd one.

            animator.GetBoneTransform(HumanBodyBones.RightUpperArm).rotation = rawVector * offset;
            */

            // UDP receiving and Deseriazile Information from Json formatted Strings
            var buffer = client.Receive(ref remote);
            string str_json = Encoding.ASCII.GetString(buffer);
            AllBodyVector3D allBodyVector3D = JsonUtility.FromJson<AllBodyVector3D>(str_json);
            Debug.Log("Received Json : " + str_json);

            //Left UpperArm #############################################################################
            upperArm_L_Vector = new Vector3(-allBodyVector3D.upperArmL.x,-allBodyVector3D.upperArmL.y,-allBodyVector3D.upperArmL.z);
            // Rotate : 回転対象の軸が向くベクトルの設定
            var rawVectorUpperArmL = Quaternion.LookRotation(upperArm_L_Vector);
            // compensate : 腕はY軸がBoneだったのでVector3.up（本来のY軸）をVector3.forward（Z軸）への回転に変換
            var offsetUpperArmL = Quaternion.LookRotation(Vector3.up,Vector3.forward); // 1st argument vector to 2nd one.
            animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).rotation = rawVectorUpperArmL * offsetUpperArmL;

            //Right UpperArm #############################################################################
            upperArm_R_Vector = new Vector3(-allBodyVector3D.upperArmR.x,-allBodyVector3D.upperArmR.y,-allBodyVector3D.upperArmR.z);
            var rawVectorUpperArmR = Quaternion.LookRotation(upperArm_R_Vector);
            var offsetUpperArmR = Quaternion.LookRotation(Vector3.up,Vector3.forward); // 1st argument vector to 2nd one.
            animator.GetBoneTransform(HumanBodyBones.RightUpperArm).rotation = rawVectorUpperArmR * offsetUpperArmR;
            
            //Left Elbow:LowerArm #############################################################################
            elbow_L_Vector = new Vector3(-allBodyVector3D.elbowL.x,-allBodyVector3D.elbowL.y,-allBodyVector3D.elbowL.z);
            var rawVectorLowerArmL = Quaternion.LookRotation(elbow_L_Vector);
            var offsetLowerArmL1 = Quaternion.LookRotation(Vector3.up,Vector3.forward); // 1st argument vector to 2nd one.
            animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).rotation = rawVectorLowerArmL * offsetLowerArmL1;

            //Right Elbow:LowerArm #############################################################################
            elbow_R_Vector = new Vector3(-allBodyVector3D.elbowR.x,-allBodyVector3D.elbowR.y,-allBodyVector3D.elbowR.z);
            var rawVectorLowerArmR = Quaternion.LookRotation(elbow_R_Vector);
            var offsetLowerArmR1 = Quaternion.LookRotation(Vector3.up,Vector3.forward); // 1st argument vector to 2nd one.
            animator.GetBoneTransform(HumanBodyBones.RightLowerArm).rotation = rawVectorLowerArmR * offsetLowerArmR1;

            //Spine #############################################################################
            // FYI : It might be replaced for Hips after the Implementation of Leg Area had fineished.
            UpperBody_Vector = new Vector3(allBodyVector3D.upperBody.x,allBodyVector3D.upperBody.y,allBodyVector3D.upperBody.z);
            // UpperBody_Vector = new Vector3((float)0,(float)0.3,(float)0.1);              
            var rawVectorUpperBody = Quaternion.LookRotation(UpperBody_Vector);
            var offsetUpperBody = Quaternion.LookRotation(Vector3.up,Vector3.forward); // 1st argument vector to 2nd one.
            var offsetUB2 = Quaternion.LookRotation(Vector3.right,Vector3.left);
            var offsetUB3 = Quaternion.LookRotation(Vector3.right,Vector3.left); // It llok stupid. There must be a way to 1step convertion.
            animator.GetBoneTransform(HumanBodyBones.Spine).rotation = rawVectorUpperBody*  offsetUpperBody * offsetUB2 * offsetUB3;

            //Head #############################################################################
            //FYI : It might be replaced for Neck
            //Spine_Vector = new Vector3(-allBodyVector3D.spine.x,-allBodyVector3D.spine.y,-allBodyVector3D.spine.z);            
            //var rawVectorSpine = Quaternion.LookRotation(Spine_Vector);
            //var offsetSpine = Quaternion.LookRotation(Vector3.up,Vector3.forward); // 1st argument vector to 2nd one.
            //nimator.GetBoneTransform(HumanBodyBones.Spin).rotation = rawVectorUpperSpine * offsetSpine;



        }

        public Vector3 something(){
            Vector3 tmp = new Vector3(0,0,0);
            return tmp;
        }

        public class AllBodyVector3D
        {
            public EachVector wristL;
            public EachVector wristR;
            public EachVector elbowL;
            public EachVector elbowR;
            public EachVector upperArmL;
            public EachVector upperArmR;
            public EachVector upperBody;
            
        }

        [Serializable]
        public class EachVector
        {
            public float x;
            public float y;
            public float z;
        }

    }
}
