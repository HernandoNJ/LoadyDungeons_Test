using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

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
	//private List<string> m_Keys = new List<string>() { "Hats", "Seasonal" };
	private List<string> m_Keys = new List<string>() { "Hats", "Fancy" };

	// Replace by the line below 
	// private AsyncOperationHandle<GameObject> m_HatLoadOpHandle;
	//private AsyncOperationHandle<IList<GameObject>> m_HatsLoadOpHandle;// *1

	// Used to set the resource location
	private AsyncOperationHandle<IList<IResourceLocation>> m_HatsLocationsOpHandle;
	
	// Used to set the asset that will be instantiated from the resource location loading
	private AsyncOperationHandle<GameObject> m_HatLoadOpHandle;


	private void Start()
	{
		//SetHat(string.Format("Hat{0:00}", GameManager.s_ActiveHat));
		// LoadInRandomHat(); // Commented by *1
		// Attempt to load all the assets across all the groups that have the "Hats" key
		//m_HatsLoadOpHandle = Addressables.LoadAssetsAsync<GameObject>(m_Keys, null, Addressables.MergeMode.Union);

		// Load only the assets that contain both the "Hats" and "Seasonal" keys
		//m_HatsLoadOpHandle = Addressables.LoadAssetsAsync<GameObject>(m_Keys, null, Addressables.MergeMode.Intersection);
		//m_HatsLoadOpHandle.Completed += OnHatsLoadComplete;

		// Get the Resources Locations without loading assets
		m_HatsLocationsOpHandle = Addressables.LoadResourceLocationsAsync(m_Keys, Addressables.MergeMode.Intersection);
		m_HatsLocationsOpHandle.Completed += OnHatLocationsLoadComplete;
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
			//LoadInRandomHat(m_HatsLoadOpHandle.Result);
			
			Addressables.Release(m_HatLoadOpHandle); // The asset is released
			LoadInRandomHat(m_HatsLocationsOpHandle.Result); // get the resource location and instantiate a new asset
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

	// LoadInRandomHat(IList<GameObject> prefabs):
	// Receives an IList with loaded prefabs(Gameobjects)

	// LoadInRandomHat(IList<IResourceLocation> resourceLocations):
	// Receives a list with a resource locations list
	private void LoadInRandomHat(IList<IResourceLocation> resourceLocations)
	{
		// Instantiate a prefab from a list of preloaded assets
		//int randomIndex = Random.Range(0, prefabs.Count);
		//GameObject randomHatPrefab = prefabs[randomIndex];
		//m_HatInstance = Instantiate(randomHatPrefab, m_HatAnchor);

		// Pick up a randon Resource Location
		int randomIndex = Random.Range(0, resourceLocations.Count);
		IResourceLocation randomHatPrefab = resourceLocations[randomIndex];

		// Load the Gameobject from the random resource location
		m_HatLoadOpHandle = Addressables.LoadAssetAsync<GameObject>(randomHatPrefab);
		m_HatLoadOpHandle.Completed += OnHatLoadComplete;
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

	// Callback after loading the Gameobject from the random resource location 
	private void OnHatLoadComplete(AsyncOperationHandle<GameObject> asyncOperationHandle)
	{
		if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
		{
			m_HatInstance = Instantiate(asyncOperationHandle.Result, m_HatAnchor);
		}
	}

	// OnHatsLoadComplete()
	// Test to check if all the hats were loaded successfully with a valid result
	// If so, iterate and log the names of all the assets with the “Hats” key 
	// And pass them along to LoadInRandomHat()
	// Collects multiple assets with its AsyncOperationHandle
	//private void OnHatsLoadComplete(AsyncOperationHandle<IList<GameObject>> asyncOperationHandle)

	// 
	private void OnHatLocationsLoadComplete(AsyncOperationHandle<IList<IResourceLocation>> asyncOperationHandle)
	{
		Debug.Log("AsyncOperationHandle Status: " + asyncOperationHandle.Status);

		if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
		{
			// results: list of loaded assets (gameobjects)
			//IList<GameObject> results = asyncOperationHandle.Result;
			//for (int i = 0; i < results.Count; i++)
			//{
			//	Debug.Log("Hat: " + results[i].name);
			//}

			// results: list of resource locations
			IList<IResourceLocation> results = asyncOperationHandle.Result;
			for (int i = 0; i < results.Count; i++)
			{
				Debug.Log("Hat: " + results[i].PrimaryKey);
			}

			LoadInRandomHat(results);
		}
	}

	private void OnDisable()
	{
		// Commented by *1
		// m_HatLoadOpHandle.Completed -= OnHatLoadComplete;
		// m_HatsLoadOpHandle.Completed -= OnHatsLoadComplete;

		m_HatLoadOpHandle.Completed -= OnHatLoadComplete;
		m_HatsLocationsOpHandle.Completed -= OnHatLocationsLoadComplete;
	}
}
