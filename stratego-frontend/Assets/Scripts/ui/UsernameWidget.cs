using System.Collections;
using TMPro;
using UnityEngine;

public class UsernameWidget : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI UsernameText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UsernameText.text = "";
    }

	private void OnEnable()
	{
		StartCoroutine(WaitForNameCoroutine());
	}

	private IEnumerator WaitForNameCoroutine()
    {
        var waitForSeconds = new WaitForSeconds(0.3f);
		string username;
        do
        {
            yield return waitForSeconds;
            username = CommData.GetInstance().GetMyUsername();
		} while (username == "");
        Debug.Log($"username {username}");
        UsernameText.text = username;
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
