using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DigitalOpus.MB.Core;

public class MB3_MeshBakerGrouper : MonoBehaviour {
	public MB3_MeshBakerGrouperCore grouper;

	//these are for getting a resonable bounds in which to draw gizmos.
	[HideInInspector] public Bounds sourceObjectBounds = new Bounds(Vector3.zero,Vector3.one);

	void OnDrawGizmosSelected(){
		if (grouper == null) return;
		if (grouper.clusterGrouper == null) return;
		if (grouper.clusterGrouper.clusterType == MB3_MeshBakerGrouperCore.ClusterType.grid){
			Vector3 cs = grouper.clusterGrouper.cellSize;
			if (cs.x <= .00001f || cs.y <= .00001f || cs.z <= .00001f) return; 
			Vector3 p = sourceObjectBounds.center - sourceObjectBounds.extents;
			Vector3 offset = grouper.clusterGrouper.origin;
			offset.x = offset.x % cs.x;
			offset.y = offset.y % cs.y;
			offset.z = offset.z % cs.z;
			//snap p to closest cell center
			Vector3 start;
			p.x = Mathf.Round((p.x) / cs.x)  * cs.x + offset.x;
			p.y = Mathf.Round((p.y) / cs.y)  * cs.y + offset.y;
			p.z = Mathf.Round((p.z) / cs.z)  * cs.z + offset.z;
			if (p.x > sourceObjectBounds.center.x - sourceObjectBounds.extents.x) p.x = p.x - cs.x;
			if (p.y > sourceObjectBounds.center.y - sourceObjectBounds.extents.y) p.y = p.y - cs.y;
			if (p.z > sourceObjectBounds.center.z - sourceObjectBounds.extents.z) p.z = p.z - cs.z;
			start = p;
			int numcells = Mathf.CeilToInt(sourceObjectBounds.size.x / cs.x + sourceObjectBounds.size.y / cs.y + sourceObjectBounds.size.z / cs.z);
			if (numcells > 200){
				Gizmos.DrawWireCube(grouper.clusterGrouper.origin + cs/2f,cs);
			} else {
				for (;p.x < sourceObjectBounds.center.x + sourceObjectBounds.extents.x; p.x += cs.x){
					p.y = start.y;
					for (;p.y < sourceObjectBounds.center.y + sourceObjectBounds.extents.y; p.y += cs.y){
						p.z = start.z;
						for (;p.z < sourceObjectBounds.center.z + sourceObjectBounds.extents.z; p.z += cs.z){
							Gizmos.DrawWireCube(p + cs/2f,cs);
						}
					}
				}
			}
		}
		if (grouper.clusterGrouper.clusterType == MB3_MeshBakerGrouperCore.ClusterType.pie){
			if (grouper.clusterGrouper.pieAxis.magnitude < .1f) return;
			if (grouper.clusterGrouper.pieNumSegments < 1) return;
			float rad = sourceObjectBounds.extents.magnitude;
			DrawCircle(grouper.clusterGrouper.pieAxis,grouper.clusterGrouper.origin,rad,24);
			Quaternion yIsUp2PieAxis = Quaternion.FromToRotation(Vector3.up,grouper.clusterGrouper.pieAxis);
			Quaternion rStep = Quaternion.AngleAxis(180f / grouper.clusterGrouper.pieNumSegments, Vector3.up);
			Vector3 r = rStep * Vector3.forward;
			for (int i = 0; i < grouper.clusterGrouper.pieNumSegments; i++){
				Vector3 rr = yIsUp2PieAxis * r;
				Gizmos.DrawLine(grouper.clusterGrouper.origin, grouper.clusterGrouper.origin + rr * rad);
				r = rStep * r;
				r = rStep * r;
			}
		}
	}

	public static void DrawCircle (Vector3 axis, Vector3 center, float radius, int subdiv) {
		Quaternion q = Quaternion.AngleAxis (360 / subdiv, axis);
		Vector3 r = new Vector3 (axis.y, -axis.x, axis.z); //should be perpendicular to axis
		r.Normalize ();
		r *= radius;
		for (int i = 0; i < subdiv+1; i ++) {
			Vector3 r2 = q * r;
			Gizmos.DrawLine (center + r, center + r2);
			r = r2;
		}
	}
}

[Serializable]
public class MB3_MeshBakerGrouperCore{
	public enum ClusterType{
		none,
		grid,
		pie,
		//voroni,
	}

	[Serializable]
	public class ClusterGrouper{

		public ClusterType clusterType;
		public Vector3 origin;
		//for grid
		public Vector3 cellSize;
		//for pie
		public int pieNumSegments = 4;
		public Vector3 pieAxis = Vector3.up;

		public Dictionary<string,List<Renderer>> FilterIntoGroups(List<GameObject> selection){
			if (clusterType == ClusterType.none){
				return FilterIntoGroupsNone(selection);
			} else if (clusterType == ClusterType.grid) {
				return FilterIntoGroupsGrid(selection);
			} else if (clusterType == ClusterType.pie){
				return FilterIntoGroupsPie(selection);
			}
			return new Dictionary<string, List<Renderer>>();
		}

		public Dictionary<string,List<Renderer>> FilterIntoGroupsNone(List<GameObject> selection)
		{
			Debug.Log ("Filtering into groups none");
			
			Dictionary<string,List<Renderer>> cell2objs = new Dictionary<string,List<Renderer>>();
			
			List<Renderer> rs = new List<Renderer>();
			for (int i = 0; i < selection.Count; i++)
			{
				rs.Add (selection[i].GetComponent<Renderer>());
			}
			
			cell2objs.Add ("MeshBaker",rs);
			return cell2objs;
		}

		public Dictionary<string,List<Renderer>> FilterIntoGroupsGrid(List<GameObject> selection){
			Dictionary<string,List<Renderer>> cell2objs = new Dictionary<string,List<Renderer>>();
			if (cellSize.x <= 0f || cellSize.y <= 0f || cellSize.z <= 0f ){
				Debug.LogError ("cellSize x,y,z must all be greater than zero.");
				return cell2objs;
			}

			Debug.Log("Collecting renderers in each cell");
			foreach (GameObject t in selection){
				GameObject go = t;
				Renderer[] mrs = go.GetComponentsInChildren<Renderer>();
				for (int j = 0; j < mrs.Length; j++){
					if (mrs[j] is MeshRenderer || mrs[j] is SkinnedMeshRenderer){
						//get the cell this gameObject is in
						Vector3 gridVector = mrs[j].transform.position;
						gridVector.x = Mathf.Floor((gridVector.x - origin.x) / cellSize.x)  * cellSize.x;
						gridVector.y = Mathf.Floor((gridVector.y - origin.y) / cellSize.y)  * cellSize.y;
						gridVector.z = Mathf.Floor((gridVector.z - origin.z) / cellSize.z)  * cellSize.z;
						List<Renderer> objs = null;
						string gridVectorStr = gridVector.ToString();
						if (cell2objs.ContainsKey(gridVectorStr)){
							objs = cell2objs[gridVectorStr];
						} else {
							objs = new List<Renderer>();
							cell2objs.Add (gridVectorStr,objs);
						}
						objs.Add (mrs[j]);
					}
				}
			}
			return cell2objs;
		}

		public Dictionary<string,List<Renderer>> FilterIntoGroupsPie(List<GameObject> selection){
			Dictionary<string,List<Renderer>> cell2objs = new Dictionary<string,List<Renderer>>();
			if (pieNumSegments == 0 ){
				Debug.LogError ("pieNumSegments must be greater than zero.");
				return cell2objs;
			}

			if (pieAxis.magnitude <= .000001f ){
				Debug.LogError ("Pie axis must have length greater than zero.");
				return cell2objs;
			}

			pieAxis.Normalize();
			Quaternion pieAxis2yIsUp = Quaternion.FromToRotation(pieAxis,Vector3.up);

			Debug.Log("Collecting renderers in each cell");
			foreach (GameObject t in selection){
				GameObject go = t;
				Renderer[] mrs = go.GetComponentsInChildren<Renderer>();
				for (int j = 0; j < mrs.Length; j++){
					if (mrs[j] is MeshRenderer || mrs[j] is SkinnedMeshRenderer){
						//get the cell this gameObject is in
						Vector3 origin2obj = mrs[j].transform.position - origin;
						origin2obj.Normalize();
						origin2obj = pieAxis2yIsUp * origin2obj;

						float d_aboutY = 0f;
						if (Mathf.Abs(origin2obj.x) < 10e-5f && Mathf.Abs(origin2obj.z) < 10e-5f){
							d_aboutY = 0f;
						} else {
							d_aboutY = Mathf.Atan2(origin2obj.z,origin2obj.x) * Mathf.Rad2Deg;
							if (d_aboutY < 0f) d_aboutY = 360f + d_aboutY;
						}

						int segment = Mathf.FloorToInt(d_aboutY / 360f * pieNumSegments);

						List<Renderer> objs = null;
						string segStr = "seg_" + segment;
						if (cell2objs.ContainsKey(segStr)){
							objs = cell2objs[segStr];
						} else {
							objs = new List<Renderer>();
							cell2objs.Add (segStr,objs);
						}
						objs.Add (mrs[j]);
					}
				}
			}
			return cell2objs;
		}
	}

	public ClusterGrouper clusterGrouper;
	public bool clusterOnLMIndex;
	
	public void DoClustering(MB3_TextureBaker tb){
		if (clusterGrouper == null){
			Debug.LogError("Cluster Grouper was null.");
			return;
		}

		//todo warn for no objects and no material bake result
		Dictionary<string,List<Renderer>> cell2objs = clusterGrouper.FilterIntoGroups(tb.GetObjectsToCombine());
		
		Debug.Log ("Found " + cell2objs.Count + " cells with Renderers. Creating bakers.");
		if (clusterOnLMIndex){
			Dictionary<string,List<Renderer>> cell2objsNew = new Dictionary<string, List<Renderer>>();
			foreach (string key in cell2objs.Keys){
				List<Renderer> gaws = cell2objs[key];
				Dictionary<int,List<Renderer>> idx2objs = GroupByLightmapIndex(gaws);
				foreach(int keyIdx in idx2objs.Keys){
					string keyNew = key + "-LM-" + keyIdx;
					cell2objsNew.Add (keyNew,idx2objs[keyIdx]);
				}
			}
			cell2objs = cell2objsNew;
		}
		foreach (string key in cell2objs.Keys){
			List<Renderer> gaws = cell2objs[key];
			AddMeshBaker (tb,key,gaws);
		}
	}

	Dictionary<int,List<Renderer>> GroupByLightmapIndex(List<Renderer> gaws){
		Dictionary<int,List<Renderer>> idx2objs = new Dictionary<int, List<Renderer>>();
		for (int i = 0; i < gaws.Count; i++){
			List<Renderer> objs = null;
			if (idx2objs.ContainsKey(gaws[i].lightmapIndex)){
				objs = idx2objs[gaws[i].lightmapIndex];
			} else {
				objs = new List<Renderer>();
				idx2objs.Add (gaws[i].lightmapIndex,objs);
			}
			objs.Add (gaws[i]);
		}
		return idx2objs;
	}

	void AddMeshBaker(MB3_TextureBaker tb, string key, List<Renderer> gaws){
		int numVerts = 0;
		for (int i = 0; i < gaws.Count; i++){
			Mesh m = MB_Utility.GetMesh(gaws[i].gameObject);
			if (m != null)
				numVerts += m.vertexCount;
		}
		
		GameObject nmb = new GameObject("MeshBaker-" + key);
		nmb.transform.position = Vector3.zero;
		MB3_MeshBakerCommon newMeshBaker;
		if (numVerts >= 65535){
			newMeshBaker = nmb.AddComponent<MB3_MultiMeshBaker>();
			newMeshBaker.useObjsToMeshFromTexBaker = false;
		} else {
			newMeshBaker = nmb.AddComponent<MB3_MeshBaker>();
			newMeshBaker.useObjsToMeshFromTexBaker = false;
		}
		newMeshBaker.textureBakeResults = tb.textureBakeResults;
		newMeshBaker.transform.parent = tb.transform;
		for (int i = 0; i < gaws.Count; i++){
			newMeshBaker.GetObjectsToCombine().Add (gaws[i].gameObject);
		}
	}
}
