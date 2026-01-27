using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Spinner : MonoBehaviour
{
	private Image SpinnerImage;
	private readonly float Speed = -30f;

	private void Awake()
	{
		SpinnerImage = GetComponent<Image>();
	}

	// Update is called once per frame
	void Update()
	{
		transform.Rotate(Vector3.forward, Speed * Time.deltaTime);
	}
}
