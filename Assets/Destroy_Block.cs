using UnityEngine;
using System.Collections;

public class Destroy_Block : MonoBehaviour {
	
	public int difficulty = 10;
	
	private bool pressed=false;
	private GameObject newblockpiece1;
	private GameObject newblockpiece2;
	private float velocity1;
	private float velocity2;
	private bool reset=false;
	
	void Update () {
		if (Input.touchCount == 3 && reset == false) {
			reset = true;
			Application.LoadLevel (0);
		} 
		else if(Input.touchCount==0){
			reset=false;
		}
		feckoff ();//rotate block pieces and fire them off screen
		GameObject[] copies=null;
		if (Input.touchCount == 1 && pressed == false) {
			pressed = true;
			RaycastHit2D destroyedblock = Physics2D.Raycast (Camera.main.ScreenToWorldPoint ((Input.GetTouch (0).position)), Vector2.zero);
			if (destroyedblock.collider != null) {
				/*
				Destroy (destroyedblock.transform.gameObject, 0.0f);//0.5f -> destroy block half a second later
				spawn(destroyedblock.transform);
				*/
				if(copies==null){
					copies=GameObject.FindGameObjectsWithTag(destroyedblock.transform.tag);
				}
				foreach(GameObject copy in copies){
					Destroy(copy);
					spawn(copy.transform);
				}
			}
		} 
		else if (Input.touchCount == 0) {
			pressed=false;
		}
	}
	
	void spawn(Transform blockposition){
		GameObject blockpiece1 = new GameObject();
		GameObject blockpiece2 = new GameObject();
		//GameObject blockpiece3 = new GameObject();
		Mesh blockpiece1mesh = new Mesh ();
		Mesh blockpiece2mesh = new Mesh ();
		//Mesh blockpiece3mesh = new Mesh ();
		
		blockpiece1mesh.Clear ();
		blockpiece1mesh.vertices=new Vector3[3]{
			new Vector3 (0, 0, 20),
			new Vector3 (Screen.width/difficulty, 0, 20),
			new Vector3 (0, Screen.height/difficulty, 20)
		};
		blockpiece1mesh.uv = new Vector2[3]{
			new Vector2 (0, 0),
			new Vector2 (1, 0),
			new Vector2 (0, 1),
			
		};
		blockpiece1mesh.triangles = new int[3]{
			0,1,2
		};
		blockpiece1mesh.RecalculateNormals ();
		MeshFilter blockpiece1meshfilter = (MeshFilter)blockpiece1.gameObject.AddComponent(typeof(MeshFilter));
		MeshRenderer blockpiece1meshrenderer = (MeshRenderer)blockpiece1.gameObject.AddComponent(typeof(MeshRenderer));
		//blockmeshrenderer.enabled = false;//get rid of that fucking annoying pink bastard
		blockpiece1meshfilter.mesh = blockpiece1mesh;
		//
		blockpiece2mesh.Clear ();
		blockpiece2mesh.vertices=new Vector3[3]{
			new Vector3 (Screen.width/difficulty, 0, 20),
			new Vector3 (Screen.width/difficulty, Screen.height/difficulty, 20),
			new Vector3 (0, Screen.height/difficulty, 20)
		};
		blockpiece2mesh.uv = new Vector2[3]{
			new Vector2 (0, 0),
			new Vector2 (1, 0),
			new Vector2 (0, 1),
			
		};
		blockpiece2mesh.triangles = new int[3]{
			0,1,2
		};
		blockpiece2mesh.RecalculateNormals ();
		MeshFilter blockpiece2meshfilter = (MeshFilter)blockpiece2.gameObject.AddComponent(typeof(MeshFilter));
		MeshRenderer blockpiece2meshrenderer = (MeshRenderer)blockpiece2.gameObject.AddComponent(typeof(MeshRenderer));
		//blockmeshrenderer.enabled = false;//get rid of that fucking annoying pink bastard
		blockpiece2meshfilter.mesh = blockpiece2mesh;
		//
		//blockpiece3mesh.Clear ();
		
		
		//spawn pieces
		newblockpiece1 = (GameObject)Instantiate(blockpiece1,blockposition.position,Quaternion.identity);
		newblockpiece1.renderer.material.color=blockposition.renderer.material.color;
		velocity1=Random.Range(200,250);
		Destroy (newblockpiece1, 0.5f);
		
		
		newblockpiece2 = (GameObject)Instantiate(blockpiece2,blockposition.position,Quaternion.identity);
		newblockpiece2.renderer.material.color=blockposition.renderer.material.color;
		velocity2=Random.Range(-200,-250);
		Destroy (newblockpiece2, 0.5f);
		
	}
	void feckoff(){
		if (newblockpiece1) {
			newblockpiece1.transform.Rotate (newblockpiece1.transform.position, velocity1 * Time.deltaTime);
			newblockpiece1.transform.Translate(Vector2.right*velocity1 * Time.deltaTime);
		}
		if (newblockpiece2) {
			newblockpiece2.transform.Rotate (newblockpiece2.transform.position,velocity2 * Time.deltaTime);
			newblockpiece2.transform.Translate(Vector2.right*velocity2 * Time.deltaTime);
		}
	}
}