using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

namespace DigitalOpus.MB.Core{
	public class MB2_TexturePacker {
		
		public MB2_LogLevel LOG_LEVEL = MB2_LogLevel.info;
		
		class PixRect{
			public int x;
			public int y;
			public int w;
			public int h;
			
			public PixRect(){}
			public PixRect(int xx, int yy, int ww, int hh){
				x = xx;
				y = yy;
				w = ww;
				h = hh;
			}
		}
		
		class Image{
			public int imgId;
			public int w;
			public int h;
			public int x;
			public int y;
	
			public Image(int id, int tw, int th, int padding, int minImageSizeX, int minImageSizeY){
				imgId = id;
				w = Mathf.Max (tw + padding * 2, minImageSizeX);
				h = Mathf.Max (th + padding * 2, minImageSizeY);
			}

			public Image(Image im){
				imgId = im.imgId;
				w = im.w;
				h = im.h;
				x = im.x;
				y = im.y;
			}
		}
		
		class ImgIDComparer : IComparer<Image>{
			public int Compare(Image x, Image y){
				if (x.imgId > y.imgId)
					return 1;
				if (x.imgId == y.imgId)
					return 0;
				return -1;
			}			
		}
		
		class ImageHeightComparer : IComparer<Image>{
			public int Compare(Image x, Image y){
				if (x.h > y.h)
					return -1;
				if (x.h == y.h)
					return 0;
				return 1;
			}
		}
	
		class ImageWidthComparer : IComparer<Image>{
			public int Compare(Image x, Image y){
				if (x.w > y.w)
					return -1;
				if (x.w == y.w)
					return 0;
				return 1;
			}
		}
	
		class ImageAreaComparer : IComparer<Image>{
			public int Compare(Image x, Image y){
				int ax = x.w * x.h;
				int ay = y.w * y.h;
				if (ax > ay)
					return -1;
				if (ax == ay)
					return 0;
				return 1;
			}
		}
		
		class ProbeResult{
			public int w;
			public int h;
			public Node root;
			public bool fitsInMaxSize;
			public float efficiency;
			public float squareness;
			
			public void Set(int ww, int hh, Node r, bool fits, float e, float sq){
				w = ww;
				h = hh;
				root = r;
				fitsInMaxSize = fits;
				efficiency = e;
				squareness = sq;
			}
			
			public float GetScore(bool doPowerOfTwoScore){
				float fitsScore = fitsInMaxSize ? 1f : 0f;
				if (doPowerOfTwoScore){
					return fitsScore * 2f + efficiency;
				} else {
					return squareness + 2 * efficiency + fitsScore;
				}
			}
		}
		
		class Node {
		    public Node[] child = new Node[2];
		    public PixRect r;
		    public Image img;
			
			bool isLeaf(){
				if (child[0] == null || child[1] == null){
					return true;
				}
				return false;
			}
			
			public Node Insert(Image im, bool handed){
				int a,b;
				if (handed){
				  a = 0;
				  b = 1;
				} else {
				  a = 1;
				  b = 0;
				}
				if (!isLeaf()){
					//try insert into first child
					Node newNode = child[a].Insert(im,handed);
					if (newNode != null)
						return newNode;
					//no room insert into second
					return child[b].Insert(im,handed);
				} else {
			        //(if there's already a lightmap here, return)
			        if (img != null) 
						return null;
			
			        //(if we're too small, return)
			        if (r.w < im.w || r.h < im.h)
			            return null;
					
			        //(if we're just right, accept)
			        if (r.w == im.w && r.h == im.h){
						img = im;
//						img.x = r.x;
//						img.y = r.y;
			            return this;
					}
			        
			        //(otherwise, gotta split this node and create some kids)
			        child[a] = new Node();
			        child[b] = new Node();
			        
			        //(decide which way to split)
			        int dw = r.w - im.w;
			        int dh = r.h - im.h;
			        
			        if (dw > dh){
			            child[a].r = new PixRect(r.x, r.y, im.w, r.h);
			            child[b].r = new PixRect(r.x + im.w, r.y, r.w - im.w, r.h);
					} else {
			            child[a].r = new PixRect(r.x, r.y, r.w, im.h);
			            child[b].r = new PixRect(r.x, r.y+ im.h, r.w, r.h - im.h);
					}
			        return child[a].Insert(im,handed);				
				}
			}
		}
		
		static void printTree(Node r, string spc){
			if (r.child[0] != null)
				printTree(r.child[0], spc + "  ");
			if (r.child[1] != null)
				printTree(r.child[1], spc + "  ");		
		}
		
		static void flattenTree(Node r, List<Image> putHere){
			if (r.img != null){
				r.img.x = r.r.x;
				r.img.y = r.r.y;				
				putHere.Add(r.img);
			}
			if (r.child[0] != null)
				flattenTree(r.child[0], putHere);
			if (r.child[1] != null)
				flattenTree(r.child[1], putHere);		
		}
		
		static void drawGizmosNode(Node r){
			Vector3 extents = new Vector3(r.r.w, r.r.h, 0);
			Vector3 pos = new Vector3(r.r.x + extents.x/2, -r.r.y - extents.y/2, 0f);
			Gizmos.DrawWireCube(pos,extents);
			if (r.img != null){
				Gizmos.color = Color.blue;
				extents = new Vector3(r.img.w, r.img.h, 0);
				pos = new Vector3(r.img.x + extents.x/2, -r.img.y - extents.y/2, 0f);
				Gizmos.DrawCube(pos,extents);
			}
			if (r.child[0] != null){
				Gizmos.color = Color.red;
				drawGizmosNode(r.child[0]);
			}
			if (r.child[1] != null){
				Gizmos.color = Color.green;
				drawGizmosNode(r.child[1]);
			}
		}
	    	
		static Texture2D createFilledTex(Color c, int w, int h){
			Texture2D t = new Texture2D(w,h);
			for (int i = 0; i < w; i++){
				for (int j = 0; j < h; j++){
					t.SetPixel(i,j,c);
				}
			}
			t.Apply();
			return t;
		}
		
		public void DrawGizmos(){
			if (bestRoot != null)
				drawGizmosNode(bestRoot.root);
		}
		
		ProbeResult bestRoot;
		public bool doPowerOfTwoTextures=true;
		
		bool Probe(Image[] imgsToAdd, int idealAtlasW, int idealAtlasH, float imgArea, int maxAtlasDim, ProbeResult pr){
			Node root = new Node();
			root.r = new PixRect(0,0,idealAtlasW,idealAtlasH);
			for (int i = 0; i < imgsToAdd.Length; i++){
				Node n = root.Insert(imgsToAdd[i],false);
				if (n == null){
					return false;
				} else if (i == imgsToAdd.Length -1){
					int usedW = 0; 
					int usedH = 0; 
					GetExtent(root,ref usedW, ref usedH);
					float efficiency,squareness;
					bool fitsInMaxDim;
					if (doPowerOfTwoTextures){
						int atlasW = Mathf.Min (CeilToNearestPowerOfTwo(usedW),maxAtlasDim);
						int atlasH = Mathf.Min (CeilToNearestPowerOfTwo(usedH),maxAtlasDim);
						if (atlasH < atlasW / 2) atlasH = atlasW / 2;
						if (atlasW < atlasH / 2) atlasW = atlasH / 2;
						fitsInMaxDim = usedW <= maxAtlasDim && usedH <= maxAtlasDim;
						float scaleW = Mathf.Max (1f,((float)usedW)/maxAtlasDim);
						float scaleH = Mathf.Max (1f,((float)usedH)/maxAtlasDim);
						float atlasArea = atlasW * scaleW * atlasH * scaleH; //area if we scaled it up to something large enough to contain images
						efficiency = 1f - (atlasArea - imgArea) / atlasArea;
						squareness = 1f; //don't care about squareness in power of two case
					} else {
						efficiency = 1f - (usedW * usedH - imgArea) / (usedW * usedH);
						if (usedW < usedH) squareness = (float) usedW / (float) usedH;
						else squareness = (float) usedH / (float) usedW;
						fitsInMaxDim = usedW <= maxAtlasDim && usedH <= maxAtlasDim;
					}
					pr.Set(usedW,usedH,root,fitsInMaxDim,efficiency,squareness);
					if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Probe success efficiency w=" + usedW + " h=" + usedH + " e=" + efficiency + " sq=" + squareness + " fits=" + fitsInMaxDim);
					return true;
				}
			}	
			Debug.LogError("Should never get here.");
			return false;
		}
		
		void GetExtent(Node r, ref int x, ref int y){
			if (r.img != null){
				if (r.r.x + r.img.w > x){
					x = r.r.x + r.img.w;
				}
				if (r.r.y + r.img.h > y) y = r.r.y + r.img.h; 
			}
			if (r.child[0] != null)
				GetExtent(r.child[0], ref x, ref y);
			if (r.child[1] != null)
				GetExtent(r.child[1], ref x, ref y);		
		}

		int StepWidthHeight(int oldVal, int step, int maxDim){
			if (doPowerOfTwoTextures && oldVal < maxDim){
				return oldVal * 2;
			} else {
				int newVal = oldVal + step;
				if (newVal > maxDim && oldVal < maxDim) newVal = maxDim;
				return newVal;
			}
		}

		public int RoundToNearestPositivePowerOfTwo(int x){
			int p = (int) Mathf.Pow(2, Mathf.RoundToInt(Mathf.Log(x)/Mathf.Log(2)));
			if (p == 0 || p == 1) p = 2;
			return p;
		}

		public int CeilToNearestPowerOfTwo(int x){
			int p = (int) Mathf.Pow(2, Mathf.Ceil(Mathf.Log(x)/Mathf.Log(2)));
			if (p == 0 || p == 1) p = 2;
			return p;
		}

		public Rect[] GetRects(List<Vector2> imgWidthHeights, int maxDimension, int padding, out int outW, out int outH){
			return _GetRects(imgWidthHeights, maxDimension, padding,2 + padding * 2, 2 + padding * 2, 2 + padding*2, 2 + padding*2, out outW, out outH, 0);
		}

		Rect[] _GetRects(List<Vector2> imgWidthHeights, int maxDimension, int padding, int minImageSizeX, int minImageSizeY, int masterImageSizeX, int masterImageSizeY, out int outW, out int outH, int recursionDepth){
			if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log (String.Format("_GetRects numImages={0}, maxDimension={1}, padding={2}, minImageSizeX={3}, minImageSizeY={4}, masterImageSizeX={5}, masterImageSizeY={6}, recursionDepth={7}",
			                                                                 imgWidthHeights.Count, maxDimension,     padding,     minImageSizeX,     minImageSizeY,     masterImageSizeX,     masterImageSizeY, recursionDepth));
			if (recursionDepth > 10){
				Debug.LogError("Maximum recursion depth reached. Couldn't find packing for these textures.");
				outW = 0;
				outH = 0;
				return new Rect[0];
			}
			float area = 0;
			int maxW = 0;
			int maxH = 0;
			Image[] imgsToAdd = new Image[imgWidthHeights.Count];
			for (int i = 0; i < imgsToAdd.Length; i++){
				Image im = imgsToAdd[i] = new Image(i,(int)imgWidthHeights[i].x, (int)imgWidthHeights[i].y, padding, minImageSizeX, minImageSizeY);
				area += im.w * im.h;
				maxW = Mathf.Max(maxW, im.w);
				maxH = Mathf.Max(maxH, im.h);
			}
			
			if ((float)maxH/(float)maxW > 2){
				if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Using height Comparer");
				Array.Sort(imgsToAdd,new ImageHeightComparer());
			}
			else if ((float)maxH/(float)maxW < .5){
				if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Using width Comparer");
				Array.Sort(imgsToAdd,new ImageWidthComparer());
			}
			else{
				if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Using area Comparer");
				Array.Sort(imgsToAdd,new ImageAreaComparer());
			}

			//explore the space to find a resonably efficient packing
			int sqrtArea = (int) Mathf.Sqrt(area);
			int idealAtlasW; 
			int idealAtlasH; 
			if (doPowerOfTwoTextures){
				idealAtlasW = idealAtlasH = RoundToNearestPositivePowerOfTwo(sqrtArea);
				if (maxW > idealAtlasW){
					idealAtlasW = CeilToNearestPowerOfTwo(idealAtlasW);
				}
				if (maxH > idealAtlasH){
					idealAtlasH = CeilToNearestPowerOfTwo(idealAtlasH);
				}
			} else {
				idealAtlasW = sqrtArea;
				idealAtlasH = sqrtArea;
				if (maxW > sqrtArea){
					idealAtlasW = maxW;
					idealAtlasH = Mathf.Max(Mathf.CeilToInt(area / maxW), maxH);
				}
				if (maxH > sqrtArea){
					idealAtlasW = Mathf.Max(Mathf.CeilToInt(area / maxH), maxW);
					idealAtlasH = maxH;
				}
			}
			if (idealAtlasW == 0) idealAtlasW = 1;
			if (idealAtlasH == 0) idealAtlasH = 1;
			int stepW = (int)(idealAtlasW * .15f);
			int stepH = (int)(idealAtlasH * .15f);
			if (stepW == 0) stepW = 1;
			if (stepH == 0) stepH = 1;
			int numWIterations=2;
			int steppedWidth = idealAtlasW;
			int steppedHeight = idealAtlasH;
			while (numWIterations >= 1 && steppedHeight < sqrtArea * 1000){	
				bool successW = false;
				numWIterations = 0;
				steppedWidth = idealAtlasW;
				while (!successW && steppedWidth < sqrtArea * 1000){	
					ProbeResult pr = new ProbeResult();
					if (LOG_LEVEL >= MB2_LogLevel.trace) Debug.Log ("Probing h=" + steppedHeight + " w=" + steppedWidth);
					if (Probe(imgsToAdd, steppedWidth, steppedHeight, area, maxDimension, pr)){
						successW = true;
						if (bestRoot == null) bestRoot = pr;
						else if (pr.GetScore(doPowerOfTwoTextures) > bestRoot.GetScore(doPowerOfTwoTextures)) bestRoot = pr;
					} else {
						numWIterations++;
						steppedWidth = StepWidthHeight(steppedWidth,stepW,maxDimension);
						if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("increasing Width h=" + steppedHeight + " w=" + steppedWidth);
					}			
				}
				steppedHeight = StepWidthHeight(steppedHeight,stepH,maxDimension);
				if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("increasing Height h=" + steppedHeight + " w=" + steppedWidth);
			}
			
			outW = 0;
			outH = 0;
			if (doPowerOfTwoTextures){
				outW = Mathf.Min (CeilToNearestPowerOfTwo(bestRoot.w),maxDimension);
				outH = Mathf.Min (CeilToNearestPowerOfTwo(bestRoot.h),maxDimension);
				if (outH < outW / 2) outH = outW / 2; //smaller dim can't be less than half larger
				if (outW < outH / 2) outW = outH / 2;
			} else {
				outW = bestRoot.w;
				outH = bestRoot.h;
			}
			if (bestRoot == null) return null;
			if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Best fit found: atlasW=" + outW + " atlasH" + outH + " w=" + bestRoot.w + " h=" + bestRoot.h + " efficiency=" + bestRoot.efficiency + " squareness=" + bestRoot.squareness + " fits in max dimension=" + bestRoot.fitsInMaxSize);

			List<Image> images = new List<Image>();
			flattenTree(bestRoot.root, images);
			images.Sort(new ImgIDComparer());
			if (images.Count != imgsToAdd.Length) Debug.LogError("Result images not the same lentgh as source");

			//scale images if too large
			int newMinSizeX = minImageSizeX;
			int	newMinSizeY = minImageSizeY;
			bool redoPacking = false;
			float padX = (float)padding / (float)outW;
			if (bestRoot.w > maxDimension){
				//float minSizeX = ((float)minImageSizeX + 1) / maxDimension;
				padX = (float)padding / (float)maxDimension;
				float scaleFactor = (float) maxDimension / (float) bestRoot.w;
				if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Packing exceeded atlas width shrinking to " + scaleFactor);
				for (int i = 0; i < images.Count; i++){
					Image im = images[i];
					if (im.w * scaleFactor < masterImageSizeX){ //check if small images will be rounded too small. If so need to redo packing forcing a larger min size
						if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log ("Small images are being scaled to zero. Will need to redo packing with larger minTexSizeX.");
						redoPacking = true;
						newMinSizeX = Mathf.CeilToInt(minImageSizeX / scaleFactor);
					}
					int right = (int) ((im.x + im.w) * scaleFactor);
					im.x = (int) (scaleFactor * im.x);
					im.w = right - im.x;
				}
				outW = maxDimension;
			}
			
			float padY = (float)padding / (float)outH;
			if (bestRoot.h > maxDimension){
				//float minSizeY = ((float)minImageSizeY + 1) / maxDimension;
				padY = (float)padding / (float)maxDimension;
				float scaleFactor = (float) maxDimension / (float) bestRoot.h;
				if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Packing exceeded atlas height shrinking to " + scaleFactor);
				for (int i = 0; i < images.Count; i++){
					Image im = images[i];
					if (im.h * scaleFactor < masterImageSizeY){ //check if small images will be rounded too small. If so need to redo packing forcing a larger min size
						if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log ("Small images are being scaled to zero. Will need to redo packing with larger minTexSizeY.");
						redoPacking = true;
						newMinSizeY = Mathf.CeilToInt(minImageSizeY / scaleFactor);
					}
					int bottom = (int) ((im.y + im.h) * scaleFactor);
					im.y = (int) (scaleFactor * im.y);
					im.h = bottom - im.y;
				}
				outH = maxDimension;
			}
			
			Rect[] rs;
			if (!redoPacking){
				rs = new Rect[images.Count];
				for (int i = 0; i < images.Count; i++){
					Image im = images[i];
					Rect r = rs[i] = new Rect((float)im.x/(float)outW + padX, 
									 (float)im.y/(float)outH + padY, 
									 (float)im.w/(float)outW - padX*2f, 
								     (float)im.h/(float)outH - padY*2f);
					if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Image: " + i + " imgID=" + im.imgId + " x=" + r.x * outW +
							   " y=" + r.y * outH + " w=" + r.width * outW +
						       " h=" + r.height * outH + " padding=" + padding);
				}
			} else {
				if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log ("==================== REDOING PACKING ================");
				bestRoot = null;
				rs = _GetRects(imgWidthHeights,maxDimension,padding,newMinSizeX,newMinSizeY,masterImageSizeX,masterImageSizeY,out outW,out outH, recursionDepth + 1);
			}
					
			if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Done GetRects");
			return rs;			
		}

		public void RunTestHarness(){
			int numTex = 32;
			int min = 126;
			int max = 2046;
			//txs[0] = new Image(0,5,800);
			//txs[1] = new Image(1,5,5000);
			
			List<Vector2> imgsToAdd = new List<Vector2>();
			for (int i = 0; i < numTex; i++){
				imgsToAdd.Add(new Vector2(UnityEngine.Random.Range(min,max), UnityEngine.Random.Range(min,max)*5));
			}

			doPowerOfTwoTextures = true;
			LOG_LEVEL = MB2_LogLevel.trace;

//			imgsToAdd.Add(new Vector2(2046,900));
//			imgsToAdd.Add(new Vector2(2046,900));
//			imgsToAdd.Add(new Vector2(2,2));
//			imgsToAdd.Add(new Vector2(2,2));
			
			int padding = 1;
			int w;
			int h;
			GetRects(imgsToAdd, 4096, padding, out w, out h);

		}	
	}
}