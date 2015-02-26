using UnityEngine;
using System.Collections;

public class Build_Wall : MonoBehaviour {
	public int difficulty=10;
	public int colorcount=5;//with higher difficulties, colorcount can be increased to include more colours, (max is 7 for now)
	void Start () {
		//lock screen rotation
		Screen.orientation = ScreenOrientation.Portrait;
		
		//scale, position main camera
		Camera.main.orthographicSize = Screen.height/2;
		Vector3 temp = Camera.main.transform.position;
		temp.x = Screen.width / 2;
		temp.y = Screen.height / 2;
		Camera.main.transform.position = temp;
		
		//create wall skeleton
		GameObject[,] wall=new GameObject[difficulty,difficulty*2];//2d array containing all blocks
		float blockwidth=Screen.width/difficulty;
		float blockheight=Screen.height/difficulty/2;
		
		//Create Block Prefab (Unbroken)
		GameObject block=new GameObject();
		Mesh blockmesh = new Mesh ();
		blockmesh.name="Block_Mesh";
		blockmesh.Clear ();//clear vertex,triangle data, etc.
		blockmesh.vertices=new Vector3[4]{
			new Vector3 (0, 0, 0),
			new Vector3 (blockwidth, 0, 0),
			new Vector3 (0, blockheight, 0),
			new Vector3 (blockwidth, blockheight, 0)
		};
		blockmesh.uv = new Vector2[4]{
			new Vector2 (0, 0),
			new Vector2 (1, 0),
			new Vector2 (0, 1),
			new Vector2 (1, 1)
		};
		blockmesh.triangles = new int[6]{0,1,2,1,3,2};
		blockmesh.RecalculateNormals ();
		MeshFilter blockmeshfilter = (MeshFilter)block.gameObject.AddComponent(typeof(MeshFilter));
		MeshRenderer blockmeshrenderer = (MeshRenderer)block.gameObject.AddComponent(typeof(MeshRenderer));
		blockmeshfilter.mesh = blockmesh;
		
		//create colour selection array
		Color32[] colours=new Color32[9]{
			new Color32(246,150,121,1),//red
			new Color32(130,202,156,1),//dark green
			new Color32(131,147,202,1),//dark blue
			new Color32(255,247,153,1),//yellow
			new Color32(095,175,153,1),//green
			new Color32(125,167,217,1),//cyan
			new Color32(161,134,190,1),//violet
			new Color32(244,154,193,1),//magenta
			new Color32(252,186,115,1)//orange
		};
		
		//default material to apply to blocks
		Material tmpmaterial = new Material(Shader.Find ("Diffuse"));
		tmpmaterial.color=new Color32(255,255,255,1);//white
		
		//store adjacent blocks
		int blockindex = 0;//give each block group unique identifiers
		GameObject tmpblock1=block;//right
		GameObject tmpblock2=block;//down
		
		GameObject oldname;//used for overwriting block names in cases when both the right and below block are the same colour
		
		for(int i=0;i<difficulty;i++){
			for(int j=0;j<difficulty*2;j++){
				wall[i,j]=(GameObject)Instantiate(block,Vector3.zero,Quaternion.identity);
				wall[i,j].transform.position=new Vector3(i*blockwidth,j*blockheight,0.0f);
				wall[i,j].renderer.material.color=colours[Random.Range(0,colorcount)];
				
				//merge blocks
				if(i>=1){tmpblock1=wall[i-1,j];}
				else{tmpblock1=null;}
				if(j>=1){tmpblock2=wall[i,j-1];}
				else{tmpblock2=null;}
				GameObject[] copies=null;
				
				//search for all bricks with name of block below, and change it to the block to the right
				if(tmpblock1!=null&&tmpblock2!=null&&wall[i,j].renderer.material.color==tmpblock1.renderer.material.color&&wall[i,j].renderer.material.color==tmpblock2.renderer.material.color){
					wall[i,j].gameObject.tag=tmpblock1.gameObject.tag;
					if(copies==null){
						copies=GameObject.FindGameObjectsWithTag(wall[i,j-1].gameObject.tag);
					}
					foreach(GameObject copy in copies){
						copy.tag=wall[i-1,j].tag;
					}
				}
				else if(tmpblock1!=null&&wall[i,j].renderer.material.color==tmpblock1.renderer.material.color){
					wall[i,j].tag=tmpblock1.tag;
					
				}//down
				else if(tmpblock2!=null&&wall[i,j].renderer.material.color==tmpblock2.renderer.material.color){
					wall[i,j].tag=tmpblock2.tag;
					
				}//right
				else{
					wall[i,j].tag=""+blockindex;
					blockindex++;
				}
				//add collider to blocks so they can be destroyed
				BoxCollider2D newcollider=wall[i,j].AddComponent<BoxCollider2D>();
				newcollider.transform.position=wall[i,j].transform.position;
				newcollider.size=wall[i,j].renderer.bounds.size;
			}
		}
		Destroy (GameObject.Find("New Game Object"),0.0f);//destroy that horrible pink bastard
	}
}
/*
Game Mode 1
Entire Screen flashes once with one colour, e.g. blue
Screen then randomly generates blocks and shows them for a short time
screen then greys out the blocks and user has to find all of the blue blocks
Points are gained for every correct selection and lost for every incorrect one
round ends when all blue blocks are found

Game Mode 2
Entire screen Flashes once with one colour, e.g. red
Screen then rapidly regenerating random blocks and player has to try and tap a red block before it changes again
(this can be fairly intense on resources with large numbers of blocks)

Game Mode 3
Entire Screen flashes with a sequence of colours e.g. red->green->blue
then blocks are generated randomly(different than before, all red blocks will be clumped together, all reds will have an adjacent red block to form one sprawling red island)
blocks are then greyed out as above, although all reds will be same shade of grey.
player must select the block groups in correct order.
note the wall can also contain colours that are not in the sequence

Game Mode 4
Screen is randomly generated again, player has to clear the screen as fast as possible, destroying all blocks

Game mode N
(combination of all of the above)
game modes are chosen at random and are distinguished by a number of beeps, e.g. two beeps for GM2
after user completes one of the above game modes (while in mode 5) the next one is randomly chosen again
(scores carry over of course)

*/