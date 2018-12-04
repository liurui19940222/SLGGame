/**
 *	\brief Hax!  DLLs cannot interpret preprocessor directives, so this class acts as a "bridge"
 */
using System;
using UnityEngine;
using System.Collections;

namespace DigitalOpus.MB.Core{

	public class MBVersionConcrete:MBVersionInterface{
		public string version(){
			return "3.7";	
		}
		
		public int GetMajorVersion(){
			#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5	
			return 3;
			#else
			return 4;
			#endif	
		}
		
		public int GetMinorVersion(){
			#if UNITY_3_0 || UNITY_3_0_0 
			return 0;
			#elif UNITY_3_1 
			return 1;
			#elif UNITY_3_2 
			return 2;
			#elif UNITY_3_3 
			return 3;
			#elif UNITY_3_4 
			return 4;
			#elif UNITY_3_5
			return 5;
			#elif UNITY_4_0 || UNITY_4_0_1
			return 0;
			#elif UNITY_4_1
			return 1;
			#elif UNITY_4_2
			return 2;
			#elif UNITY_4_3
			return 3;
			#elif UNITY_4_4
			return 4;
			#elif UNITY_4_5
			return 5;
			#else
			return 0;
			#endif	
		}
		
		public bool GetActive(GameObject go){
			#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5	
			return go.active;
			#else
			return go.activeInHierarchy;
			#endif			
		}
		
		public void SetActive(GameObject go, bool isActive){
			#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5	
			go.active = isActive;
			#else
			go.SetActive(isActive);
			#endif
		}
		
		public void SetActiveRecursively(GameObject go, bool isActive){
			#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5	
			go.SetActiveRecursively(isActive);
			#else
			go.SetActive(isActive);
			#endif
		}
		
		public UnityEngine.Object[] FindSceneObjectsOfType(Type t){
			#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5	
			return GameObject.FindSceneObjectsOfType(t);
			#else
			return GameObject.FindObjectsOfType(t);
			#endif				
		}
		
		public bool IsRunningAndMeshNotReadWriteable(Mesh m){
			if (Application.isPlaying){
				#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5	
				return false;
				#else
				return !m.isReadable;
				#endif
			} else {
				return false;
			}
		}

		Vector2 _HALF_UV = new Vector2(.5f, .5f);
		public Vector2[] GetMeshUV1s(Mesh m, MB2_LogLevel LOG_LEVEL)
		{
			Vector2[] uv;
			#if (UNITY_4_6 || UNITY_4_5 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4_0_1 || UNITY_4_0 || UNITY_3_5)
			uv = m.uv1;

			#else
			if (LOG_LEVEL >= MB2_LogLevel.warn) MB2_Log.LogDebug("UV1 does not exist in Unity 5+");
			uv = m.uv;
			#endif
			if (uv.Length == 0){
				if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Mesh " + m + " has no uv1s. Generating");
				if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Mesh " + m + " didn't have uv1s. Generating uv1s.");			
				uv = new Vector2[m.vertexCount];
				for (int i = 0; i < uv.Length; i++){uv[i] = _HALF_UV;}
			}		
			return uv;
		}

		public void MeshClear(Mesh m, bool t){
			#if UNITY_3_5
				m.Clear();
			#else
				m.Clear(t);
			#endif
		}

		public void MeshAssignUV1(Mesh m, Vector2[] uv1s){
			#if (UNITY_4_6 || UNITY_4_5 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4_0_1 || UNITY_4_0 || UNITY_3_5)
			m.uv1 = uv1s;
			#else
			Debug.LogWarning("UV1 was checked but UV1 does not exist in Unity 5+");
			#endif
		}

		public Vector3 GetLightmapTilingOffset(Renderer r){
			#if (UNITY_4_6 || UNITY_4_5 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4_0_1 || UNITY_4_0 || UNITY_3_5)
			  return r.lightmapTilingOffset ;
			#else
			  return r.lightmapScaleOffset ;
			#endif
		}

		public Transform[] GetBones(Renderer r){
			if (r is SkinnedMeshRenderer){
				Transform[] bone = ((SkinnedMeshRenderer)r).bones;
				#if UNITY_EDITOR
				if (bone.Length == 0){
					Mesh m = ((SkinnedMeshRenderer)r).sharedMesh;
					if (m.bindposes.Length != bone.Length) Debug.LogError("SkinnedMesh (" + r.gameObject + ") in the list of objects to combine has no bones. Check that 'optimize game object' is not checked in the 'Rig' tab of the asset importer. Mesh Baker cannot combine optimized skinned meshes because the bones are not available.");
				}
				#endif
				return bone;	
			} else if (r is MeshRenderer){
				Transform[] bone = new Transform[1];
				bone[0] = r.transform;
				return bone;
			} else {
				Debug.LogError("Could not getBones. Object does not have a renderer");
				return null;
			}
		}
	}
}