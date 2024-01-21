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

	private AsyncOperationHandle<GameObject> m_HatLoadOpHandle;

	private void Start()
	{
		//SetHat(string.Format("Hat{0:00}", GameManager.s_ActiveHat));
		LoadInRandomHat();
	}

	private void Update()
	{
		if (Input.GetMouseButtonUp(1))
		{
			Destroy(m_HatInstance);
			Addressables.ReleaseInstance(m_HatLoadOpHandle);

			LoadInRandomHat();
		}
	}

	private void LoadInRandomHat()
	{
		int randomIndex = Random.Range(0, 6);
		string hatAddress = string.Format("Hat{0:00}", randomIndex);

		m_HatLoadOpHandle = Addressables.LoadAssetAsync<GameObject>(hatAddress);
		m_HatLoadOpHandle.Completed += OnHatLoadComplete;
	}

	//public void SetHat(string hatKey)
	//{
	//	// Check if the value chosen in the object picker is a valid address
	//	if (!m_HatAssetReference.RuntimeKeyIsValid()) return; 

	//	m_HatLoadOpHandle = m_HatAssetReference.LoadAssetAsync<GameObject>();
	//	m_HatLoadOpHandle.Completed += OnHatLoadComplete;
	//}

	private void OnHatLoadComplete(AsyncOperationHandle<GameObject> asyncOperationHandle)
	{
		if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
		{
			//Instantiate(asyncOperationHandle.Result, m_HatAnchor);
			m_HatInstance = Instantiate(asyncOperationHandle.Result, m_HatAnchor);
		}
	}

	private void OnDisable()
	{
		m_HatLoadOpHandle.Completed -= OnHatLoadComplete;
	}
}
