using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Vector3 shoulder_vector = new Vector3(0,0,0);
        private Transform _target;
        Transform target_transform;
        private Vector3 _offset = Vector3.up;
        Vector3 RHPVECTOR;


        private void Start()
        {
            Debug.Log("Start");
            animator = GetComponent<Animator>();
            //animator.GetBoneTransform(HumanBodyBones.RightUpperArm).LookAt(new Vector3(100,100,100));
            GameObject target = GameObject.Find("Sphere");
            target_transform = target.transform;
            Transform hip_T = animator.GetBoneTransform(HumanBodyBones.Hips);
            //hip_T.position = new Vector3(0,0,0);
            //animator.GetBoneTransform(HumanBodyBones.RightUpperArm).LookAt(target_transform);
            
           
        }

        private void Update()
        {
           // _animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).Rotate(new Vector3(0,0,0.1f));
            frame_counter = frame_counter +1;
            rotate_global_deg = 30*Math.Sin((double)frame_counter/1000);
            xdir = (float)Math.Cos(rotate_global_deg * 3.14/180);
            ydir = 0;
            zdir = (float)Math.Sin(rotate_global_deg * 3.14/180);
            RHPVECTOR = new Vector3(xdir,ydir,zdir);
            
            // Calculate vector to target from selected bone.
            var dir = RHPVECTOR;
            
            // Rotate : 回転対象の軸が向くベクトルの設定
            var rawVector = Quaternion.LookRotation(dir);
            // compensate : 腕はY軸がBoneだったのでVector3.up（本来のY軸）をVector3.forward（Z軸）への回転に変換
            var offset = Quaternion.LookRotation(Vector3.up,Vector3.forward); // 1st argument vector to 2nd one.

            animator.GetBoneTransform(HumanBodyBones.RightUpperArm).rotation = rawVector * offset;

            Debug.Log(frame_counter + ", " + rotate_global_deg + " Look for " + xdir + "," + zdir);
        }
    }
}