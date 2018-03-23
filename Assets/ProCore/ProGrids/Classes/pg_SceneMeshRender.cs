#if UNITY_EDITOR
using UnityEngine;

/**
 * Despite the MonoBehaviour inheritance, this is an Editor-only script.
 */
namespace Assets.ProCore.ProGrids.Classes
{
	[ExecuteInEditMode]
	public class pg_SceneMeshRender : UnityEngine.MonoBehaviour
	{
		// HideFlags.DontSaveInEditor isn't exposed for whatever reason, so do the bit math on ints 
		// and just cast to HideFlags.
		// HideFlags.HideInHierarchy | HideFlags.DontSaveInEditor | HideFlags.NotEditable
		HideFlags SceneCameraHideFlags = (HideFlags) (1 | 4 | 8);

		public Mesh mesh;
		public Material material;

		void OnDestroy()
		{
			if(mesh) DestroyImmediate(mesh);
			if(material) DestroyImmediate(material);
		}

		void OnRenderObject()
		{
			// instead of relying on 'SceneCamera' string comparison, check if the hideflags match.
			// this could probably even just check for one bit match, since chances are that any 
			// game view camera isn't going to have hideflags set.
			if( (Camera.current.gameObject.hideFlags & SceneCameraHideFlags) != SceneCameraHideFlags || Camera.current.name != "SceneCamera" )
				return;

			if(material == null || mesh == null)
			{
			
				GameObject.DestroyImmediate(this.gameObject);
				// Debug.Log("NULL MESH || MATERIAL");
				return;
			}

			material.SetPass(0);
			Graphics.DrawMeshNow(mesh, Vector3.zero, Quaternion.identity, 0);
		}
	}
}
#endif
