using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// Used for the Hat selection logic
public class PlayerConfigurator : MonoBehaviour
{
	// the input Address is a string that isn't validated as an address until you make the load request
	// You could enter anything in this property and cause an error
	// Make sure that the text matches an address in the Groups window.
	// [SerializeField] private string m_Address;

	// Instead of a direct reference, m_HatAssetReference holds the
	// GUID of the AssetReference used by the Addressable System
	// To store the object to be retrieved at runtime
	// --- AssetReference works with any asset marked as addressable
	// To refine or restric the type to only prefabs (GameObjects)
	// Change the type to AssetReferenceGameObject
	//[SerializeField] private AssetReferenceGameObject m_HatAssetReference;

	private GameObject m_HatInstance;
	[SerializeField] private Transform m_HatAnchor;

	// *1 Loading assets by label
	private List<string> m_Keys = new List<string>() { "Hats" };

	// Replace by the line below 
	// private AsyncOperationHandle<GameObject> m_HatLoadOpHandle;
	private AsyncOperationHandle<IList<GameObject>> m_HatsLoadOpHandle;// *1

	private void Start()
	{
		//SetHat(string.Format("Hat{0:00}", GameManager.s_ActiveHat));
		// LoadInRandomHat(); // Commented by *1
		// Attempt to load all the assets across all the groups that have the "Hats" key
		m_HatsLoadOpHandle = Addressables.LoadAssetsAsync<GameObject>(m_Keys, null, Addressables.MergeMode.Union);
		m_HatsLoadOpHandle.Completed += OnHatsLoadComplete;
	}

	private void Update()
	{
		if (Input.GetMouseButtonUp(1))
		{
			// Commented by *1
			//Destroy(m_HatInstance);
			//Addressables.ReleaseInstance(m_HatLoadOpHandle);
			//LoadInRandomHat();

			// Here, we are still destroying the instance of the hat prefab,
			// But instead of releasing the prefab, we keep all the hat assets in memory
			// We avoid releasing because the result of the load operation wasn’t one asset but multiple
			Destroy(m_HatInstance);
			LoadInRandomHat(m_HatsLoadOpHandle.Result);
		}
	}

	// Chooses one of the prefabs at random
	// Commented by *1
	// private void LoadInRandomHat()
	// {
	// 	int randomIndex = Random.Range(0, 6);
	// 	string hatAddress = string.Format("Hat{0:00}", randomIndex);

	// 	m_HatLoadOpHandle = Addressables.LoadAssetAsync<GameObject>(hatAddress);
	// 	m_HatLoadOpHandle.Completed += OnHatLoadComplete;
	// }

	// Receives an IList with loaded prefabs(Gameobjects)
	private void LoadInRandomHat(IList<GameObject> prefabs)
	{
		int randomIndex = Random.Range(0, prefabs.Count);
		GameObject randomHatPrefab = prefabs[randomIndex];
		m_HatInstance = Instantiate(randomHatPrefab, m_HatAnchor);
	}

	//public void SetHat(string hatKey)
	//{
	//	// Check if the value chosen in the object picker is a valid address
	//	if (!m_HatAssetReference.RuntimeKeyIsValid()) return; 

	//	m_HatLoadOpHandle = m_HatAssetReference.LoadAssetAsync<GameObject>();
	//	m_HatLoadOpHandle.Completed += OnHatLoadComplete;
	//}

	// *1
	// private void OnHatLoadComplete(AsyncOperationHandle<GameObject> asyncOperationHandle)
	// {
	// 	if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
	// 	{
	// 		//Instantiate(asyncOperationHandle.Result, m_HatAnchor);
	// 		m_HatInstance = Instantiate(asyncOperationHandle.Result, m_HatAnchor);
	// 	}
	// }

	// Test to check if all the hats were loaded successfully with a valid result
	// If so, iterate and log the names of all the assets with the “Hats” key 
	// And pass them along to LoadInRandomHat()
	private void OnHatsLoadComplete(AsyncOperationHandle<IList<GameObject>> asyncOperationHandle)
	{
		Debug.Log("AsyncOperationHandle Status: " + asyncOperationHandle.Status);

		if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
		{
			IList<GameObject> results = asyncOperationHandle.Result;
			for (int i = 0; i < results.Count; i++)
			{
				Debug.Log("Hat: " + results[i].name);
			}
			LoadInRandomHat(results);
		}
	}

	private void OnDisable()
	{
		// Commented by *1
		// m_HatLoadOpHandle.Completed -= OnHatLoadComplete;
		m_HatsLoadOpHandle.Completed -= OnHatsLoadComplete;
	}
}
