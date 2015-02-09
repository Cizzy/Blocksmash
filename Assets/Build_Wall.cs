using UnityEngine;
using System.Collections;

public class Build_Wall : MonoBehaviour {

	public int difficulty=10;
	//public Camera myCam;

	//create 2d array of block objects for referencing later
	private GameObject[,] blockarray;

	void Start () {
		//scale screen
		Screen.orientation = ScreenOrientation.Portrait;

		//scale camera
		Camera.main.orthographicSize = Screen.height/2;

		//Center Camera
		Vector3 temp = Camera.main.transform.position;
		temp.x = Screen.width / 2;
		temp.y = Screen.height / 2;
		Camera.main.transform.position = temp;

		//Create Block Prefab (Unbroken)
		GameObject block=new GameObject();
		//MeshRenderer meshrenderer=block.GetComponent<MeshRenderer>();
		Mesh blockmesh = new Mesh ();
		blockmesh.name="Block_Mesh";
		blockmesh.Clear ();//clear vertex,triangle data, etc.
		blockmesh.vertices=new Vector3[4]{
			new Vector3 (0, 0, 20),
			new Vector3 (Screen.width/difficulty, 0, 20),
			new Vector3 (0, Screen.height/difficulty, 20),
			new Vector3 (Screen.width/difficulty, Screen.height/difficulty, 20)
		};
		blockmesh.uv = new Vector2[4]{
			new Vector2 (0, 0),
			new Vector2 (1, 0),
			new Vector2 (0, 1),
			new Vector2 (1, 1)
		};
		blockmesh.triangles = new int[6]{
			0,1,2,1,3,2
		};
		blockmesh.RecalculateNormals ();
		MeshFilter blockmeshfilter = (MeshFilter)block.gameObject.AddComponent(typeof(MeshFilter));
		MeshRenderer blockmeshrenderer = (MeshRenderer)block.gameObject.AddComponent(typeof(MeshRenderer));
		//blockmeshrenderer.enabled = false;//get rid of that fucking annoying pink bastard
		blockmeshfilter.mesh = blockmesh;

		//create colour selection array
		int colorcount = 7;//with higher difficulties, colorcount can be increased to include more colours, (max is 7 for now)
		Color[] colours=new Color[7]{
			Color.white,
			Color.red,
			Color.blue,
			Color.green,
			Color.yellow,
			Color.magenta,
			Color.cyan,
		};

		//default material to apply to blocks
		Material tmpmaterial = new Material(Shader.Find ("Diffuse"));
		tmpmaterial.color= Color.black;

		//Build Wall
		float blockwidth = Screen.width / difficulty;
		float blockheight = Screen.height / difficulty;

		//blockmeshrenderer.enabled = true;

		//GameObject tmpblock1=new GameObject();
		//GameObject tmpblock2=new GameObject();

		//give each block unique identifiers (used for merging blocks, can also be used for stats)
		int blockindex=0;

		Vector3 tmpposition1;
		Vector3 tmpposition2;

		//two temporary blocks used for merging blocks of the same colour into one (used as they are created. one to the right, one below)
		GameObject tmpblock1=new GameObject();
		GameObject tmpblock2=new GameObject();

		Color tmpcolor1=Color.white;
		Color tmpcolor2=Color.white;

		for (int i=0; i<difficulty; i++) {
			for(int j=0; j<difficulty; j++){

				//spawn block
				GameObject newBlock = (GameObject)Instantiate(block,new Vector3(i*blockwidth,j*blockheight,-20.0f),Quaternion.identity);
				newBlock.renderer.material.color=colours[Random.Range(0,colorcount)];
				//newBlock.name=(""+(blockindex++));//give each block a unique identifier

				//merge blocks (to the right)
				tmpposition1=newBlock.transform.position;
				//Debug.Log("original block1 location", tmpposition1);
				tmpposition1.x-=Screen.width/difficulty;
				//Debug.Log("block (to the right of original) location", tmpposition1);
				Collider[] hitColliders1 = Physics.OverlapSphere(tmpposition1, Screen.width/difficulty/4);
				if(hitColliders1.Length>=1){
					tmpblock1=hitColliders1[0].transform.parent.gameObject;
					tmpcolor1=hitColliders1[0].transform.parent.gameObject.renderer.material.color;
				}
				//merge blocks (below)
				tmpposition2=newBlock.transform.position;
				//Debug.Log("original block2 location, tmpposition2",tmpposition2);
				tmpposition2.y-=Screen.height/difficulty;
				//Debug.Log("block (below original) location", tmpposition2);
				Collider[] hitColliders2 = Physics.OverlapSphere(tmpposition2, Screen.width/difficulty/4);
				if(hitColliders2.Length>=1){
					tmpblock2=hitColliders2[0].transform.parent.gameObject;
					tmpcolor2=hitColliders2[0].transform.parent.gameObject.renderer.material.color;
				}
				if(newBlock.renderer.material.color==tmpcolor1){
					newBlock.name=tmpblock1.name;
				}
				else if(newBlock.renderer.material.color==tmpcolor2){
					newBlock.name=tmpblock2.name;
				}
				else{
					newBlock.name=(""+(blockindex++));//give each block a unique identifier
				}
				//blockarray[i,j]=newBlock;
				//combine blocks of same colour
				//tmpblock1=blockarray[i-1,j];
				//tmpblock2=blockarray[i,j-1];

				//if(newBlock.renderer.material.color==tmpblock1.renderer.material.color&&i>=1){
				//	newBlock.name=tmpblock1.name;
				//}
				//else if(newBlock.renderer.material.color==tmpblock2.renderer.material.color&&j>=1){
				//	newBlock.name=tmpblock2.name;
				//}
				//else{
				//	newBlock.name=(""+(blockindex++));//give each block a unique identifier
				//}
				BoxCollider2D newcollider=newBlock.AddComponent<BoxCollider2D>();
				newcollider.transform.position=newBlock.transform.position;
				newcollider.size=newBlock.renderer.bounds.size;
			}
		}
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