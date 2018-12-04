/**
 *	\brief Hax!  DLLs cannot interpret preprocessor directives, so this class acts as a "bridge"
 */
using System;
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace DigitalOpus.MB.Core{

	public interface MBVersionEditorInterface{
		string GetPlatformString();
		int GetMaximumAtlasDimension();
	}
	
	public class MBVersionEditor
	{
		private static MBVersionEditorInterface _MBVersion;

		public static string GetPlatformString(){
			if (_MBVersion == null) _MBVersion = (MBVersionEditorInterface) Activator.CreateInstance(Type.GetType("DigitalOpus.MB.Core.MBVersionEditorConcrete,Assembly-CSharp-Editor"));
			return _MBVersion.GetPlatformString();
		}

		public static int GetMaximumAtlasDimension(){
			if (_MBVersion == null) _MBVersion = (MBVersionEditorInterface) Activator.CreateInstance(Type.GetType("DigitalOpus.MB.Core.MBVersionEditorConcrete,Assembly-CSharp-Editor"));
			return _MBVersion.GetMaximumAtlasDimension();
		}
	}
}