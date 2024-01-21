using UnityEngine;

public class Test : MonoBehaviour
{
	[SerializeField] private GameObject obj1;

	private void Start()
	{
		TestGenerics1<GameObject> testGenerics1 = new TestGenerics1<GameObject>();
		testGenerics1.UpdateItem(obj1);
	
	}
}
