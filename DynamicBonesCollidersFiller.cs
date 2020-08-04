using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DynamicBonesCollidersFiller : EditorWindow
{
    //Created By Plague <3
    //Copyright Reserved
    [MenuItem("GameObject/Set DynamicBones To Have All Colliders", false, -10)]
    static void Set()
    {
        //Check If The User Has Even Got A GameObject Selected - Prevents NullReferenceExceptions Due To Such.
        if (Selection.gameObjects.Length == 0)
        {
            Debug.Log("You Have No GameObjects Selected!");
            return;
        }

        //Define Object Reference
        GameObject RootArmature = null;

        //Define Variables
        int DynBonesFound = 0;

        //Retrieve The Armature Of The Avatar
            //Get All The Root GameObjects In The Scene
        foreach (GameObject obj in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            //Enumerate The First Child GameObject Of Each GameObject In The Root Of The Scene
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                //Object Reference
                GameObject obj2 = obj.transform.GetChild(i).gameObject;

                //Check If Found GameObject Is The Armature
                if (obj2.name.Contains("Armature"))
                    //If So, Add It To The Object Reference For Later Use
                    RootArmature = obj2;
            }
        }

        //If No Armature Was Found, Don't Continue
        if (RootArmature == null)
        {
            Debug.LogError("Armature == null!");

            return;
        }

        //Recursively Check Every GameObject In The Armature For DynamicBoneColliders
        Helpers.CheckTransform(RootArmature.transform);

        //Check If No DynamicBoneColliders Were Found - Prevents NullReferenceExceptions Due To Such.
        if (Helpers.DynamicBoneColliders == null)
        {
            Debug.LogError("Helpers.DynamicBoneColliders == null!");

            return;
        }

        //Enumerate Selected GameObjects
        foreach (var CurrentGameObject in Selection.gameObjects)
        {
            //Ignore Empty GameObjects
            if (CurrentGameObject == null)
				continue;
			
            //Enumerate DynamicBones On GameObject
            foreach (DynamicBone DynBone in CurrentGameObject.GetComponents<DynamicBone>())
            {
                //Ignore Invalid DynamicBone Components In GameObject
				if (DynBone == null)
					continue;

                //Raise DynBonesFound By 1
                DynBonesFound++;

                //Set Colliders On DynamicBone
                DynBone.m_Colliders = Helpers.DynamicBoneColliders;
            }
        }

        //If No DynamicBones Were Found And Therefore Effected, Print To Console
        if (DynBonesFound == 0)
        {
            Debug.Log("No Dynamic Bones Were Found So None Were Effected!");
        }
    }

    public class Helpers
    {
        //Define Object Reference
        public static List<DynamicBoneColliderBase> DynamicBoneColliders;

        //Init
        public static void CheckTransform(Transform transform)
        {
            //Reset Object Reference
            DynamicBoneColliders = new List<DynamicBoneColliderBase>();

            //Don't Act On A Invalid Transform
            if (transform == null)
            {
                return;
            }

            //Call Recursive Method
            GetChildren(transform);
        }

        //Recursive Method
        public static void GetChildren(Transform transform)
        {
            //If There Is A DynamicBoneCollider On This Transform
            if (transform.GetComponent<DynamicBoneCollider>())
                //Add It To List For Use Later
                DynamicBoneColliders.Add(transform.GetComponent<DynamicBoneCollider>());

            //Recursive Re-Call
            for (int i = 0; i < transform.childCount; i++)
            {
                GetChildren(transform.GetChild(i));
            }
        }
    }
}
