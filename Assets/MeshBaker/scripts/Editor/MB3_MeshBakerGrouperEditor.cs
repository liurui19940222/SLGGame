//----------------------------------------------
//            MeshBaker
// Copyright © 2011-2012 Ian Deane
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using DigitalOpus.MB.Core;
using UnityEditor;


[CustomEditor(typeof(MB3_MeshBakerGrouper))]
public class MB3_MeshBakerGrouperEditor : Editor {

		long lastBoundsCheckRefreshTime = 0;

		static GUIContent gc_ClusterType = new GUIContent("Cluster Type","The scene will be divided cells. Meshes in each cell will be grouped into a single mesh baker");
		static GUIContent gc_GridOrigin = new GUIContent("Origin","The scene will be divided into of cells. Meshes in each cell will be grouped into a single baker. This sets the origin for the clustering.");
		static GUIContent gc_CellSize = new GUIContent("Cell Size", "The scene will be divided into a grid of cells. Meshes in each cell will be grouped into a single baker. This sets the size of the cells.");
		static GUIContent gc_ClusterOnLMIndex  = new GUIContent("Group By Lightmap Index", "Meshes sharing a lightmap index will be grouped together.");
		static GUIContent gc_NumSegements  = new GUIContent("Num Pie Segments", "Number of segments/slices in the pie.");
		static GUIContent gc_PieAxis  = new GUIContent("Pie Axis", "Scene will be divided into segments about this axis.");

		private SerializedObject grouper;
		private SerializedProperty clusterType, gridOrigin, cellSize, clusterOnLMIndex, numSegments, pieAxis;	
		
		public void OnEnable (){
			lastBoundsCheckRefreshTime = 0;
			grouper = new SerializedObject(target);
			SerializedProperty grp = grouper.FindProperty("grouper");
			SerializedProperty clusterGrouper = grp.FindPropertyRelative("clusterGrouper");
			clusterType = clusterGrouper.FindPropertyRelative("clusterType");
			gridOrigin = clusterGrouper.FindPropertyRelative("origin");
			cellSize = clusterGrouper.FindPropertyRelative("cellSize");
			clusterOnLMIndex = grp.FindPropertyRelative("clusterOnLMIndex");
			numSegments = clusterGrouper.FindPropertyRelative("pieNumSegments");
			pieAxis = clusterGrouper.FindPropertyRelative("pieAxis");
		}

		public override void OnInspectorGUI(){
			MB3_MeshBakerGrouper tbg = (MB3_MeshBakerGrouper) target;
			MB3_TextureBaker tb = ((MB3_MeshBakerGrouper)target).GetComponent<MB3_TextureBaker>();
			DrawGrouperInspector();
			if (GUILayout.Button("Generate Mesh Bakers")){
				if (tb == null){
					Debug.LogError("There must be an MB3_TextureBaker attached to this game object.");
					return;
				}
				if (tb.GetObjectsToCombine().Count == 0){
					Debug.LogError("The MB3_MeshBakerGrouper creates clusters based on the objects to combine in the MB3_TextureBaker component. There were no objects in this list.");
					return;
				}
				if (tb.transform.childCount > 0){
					Debug.LogWarning("This MB3_TextureBaker had some existing child objects. You may want to delete these before 'Generating Mesh Bakers' since your source objects may be included in the List Of Objects To Combine of multiple MeshBaker objects.");
				}
				if (tb != null){
					((MB3_MeshBakerGrouper) target).grouper.DoClustering(tb);
				} else {
					Debug.LogError("MB3_MeshBakerGrouper needs to be attached to an MB3_TextureBaker");
				}
			}
			if (GUILayout.Button ("Bake All Child MeshBakers")){
				try{
					MB3_MeshBakerCommon[] mBakers = tbg.GetComponentsInChildren<MB3_MeshBakerCommon>();
					for (int i = 0; i < mBakers.Length; i++){
						if (mBakers[i].textureBakeResults != null){
							MB3_MeshBakerEditorFunctions.BakeIntoCombined(mBakers[i]);	
						}
					}					
				} catch (Exception e) {
					Debug.LogError(e);
				}finally{
					EditorUtility.ClearProgressBar();
				}
			}
			string buttonTextEnableRenderers = "Disable Renderers On All Child MeshBakers";
			bool enableRenderers = false;
			if (tb != null && tb.GetObjectsToCombine().Count > 0){
				GameObject go = tb.GetObjectsToCombine()[0];
				if (go != null && go.GetComponent<Renderer>() != null && go.GetComponent<Renderer>().enabled == false){
					buttonTextEnableRenderers = "Enable Renderers On All Child MeshBakers";
					enableRenderers = true;
				}
			}
			if (GUILayout.Button (buttonTextEnableRenderers)){
				try{
					MB3_MeshBakerCommon[] mBakers = tbg.GetComponentsInChildren<MB3_MeshBakerCommon>();
					for (int i = 0; i < mBakers.Length; i++){
						for (int j = 0; j < mBakers[i].GetObjectsToCombine().Count; j++){
							GameObject go = mBakers[i].GetObjectsToCombine()[j];
							if (go != null && go.GetComponent<Renderer>() != null){
								go.GetComponent<Renderer>().enabled = enableRenderers;
							}
						}
					}					
				} catch (Exception e) {
					Debug.LogError(e);
				}finally{
					EditorUtility.ClearProgressBar();
				}				
			}
			if (DateTime.UtcNow.Ticks - lastBoundsCheckRefreshTime > 10000000 && tb != null){
				List<GameObject> gos = tb.GetObjectsToCombine();
				Bounds b = new Bounds(Vector3.zero,Vector3.one);
				if (gos.Count > 0 && gos[0] != null && gos[0].GetComponent<Renderer>() != null){
					b = gos[0].GetComponent<Renderer>().bounds;
				}
				for (int i = 0; i < gos.Count; i++){
					if (gos[i] != null && gos[i].GetComponent<Renderer>() != null){
						b.Encapsulate(gos[i].GetComponent<Renderer>().bounds);
					}
				}	
				tbg.sourceObjectBounds = b;
				lastBoundsCheckRefreshTime = DateTime.UtcNow.Ticks;
			}
			grouper.ApplyModifiedProperties();
		}

		public void DrawGrouperInspector(){
			EditorGUILayout.HelpBox("This component helps you group meshes that are close together so they can be combined together." +
								" It generates multiple MB3_MeshBaker objects from the List Of Objects to be combined in the MB3_TextureBaker component." +
		                        " Objects that are close together will be grouped together and added to a new child MB3_MeshBaker object.",MessageType.Info);
			//if (grouper == null || grouper.clusterGrouper == null) return;
			//MB3_MeshBakerGrouperCore.ClusterGrouper cg = grouper.clusterGrouper;
			EditorGUILayout.PropertyField(clusterType,gc_ClusterType);
			//cg.clusterType = (MB3_MeshBakerGrouperCore.ClusterType) EditorGUILayout.EnumPopup(gc_ClusterType,cg.clusterType);
			if (clusterType.enumValueIndex == (int) MB3_MeshBakerGrouperCore.ClusterType.grid){
				EditorGUILayout.PropertyField(gridOrigin,gc_GridOrigin);
				EditorGUILayout.PropertyField(cellSize,gc_CellSize);
			} else if (clusterType.enumValueIndex == (int) MB3_MeshBakerGrouperCore.ClusterType.pie){
				EditorGUILayout.PropertyField(gridOrigin,gc_GridOrigin);
				EditorGUILayout.PropertyField(numSegments,gc_NumSegements);
				EditorGUILayout.PropertyField(pieAxis,gc_PieAxis);
			}
			EditorGUILayout.PropertyField(clusterOnLMIndex,gc_ClusterOnLMIndex);
		}
}
