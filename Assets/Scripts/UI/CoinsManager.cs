using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using Tools;

public class CoinsManager : MonoBehaviour
{
	//References
	[Header ("UI references")]
	[SerializeField] TMP_Text coinUIText;
	[SerializeField] GameObject animatedCoinPrefab;
	[SerializeField] RectTransform target;
	[SerializeField] RectTransform parent;
	[SerializeField] playOneShotAnimation targetImage;
	[Space]
	[Header ("Available coins : (coins to pool)")]
	[SerializeField] int maxCoins;
	Queue<GameObject> coinsQueue = new Queue<GameObject> ();


	[Space]
	[Header ("Animation settings")]
	[SerializeField] [Range (0.5f, 0.9f)] float minAnimDuration;
	[SerializeField] [Range (0.9f, 2f)] float maxAnimDuration;

	[SerializeField] Ease easeType;
	[SerializeField] float spread;

	Vector3 targetPosition;


	private int _c = 0;

	public int Coins {
		get{ return _c; }
		set {
			_c = value;
			//update UI text whenever "Coins" variable is changed
			coinUIText.text = Coins.ToString ();
		}
	}

	void Awake ()
	{
		targetPosition = TweenHelper.SwitchToRectTransform(target, parent);

		//prepare pool
		PrepareCoins ();
	}

	void PrepareCoins ()
	{
		GameObject coin;
		for (int i = 0; i < maxCoins; i++) {
			coin = Instantiate (animatedCoinPrefab);
			coin.transform.parent = parent;
			coin.SetActive (false);
			coinsQueue.Enqueue (coin);
		}
	}

	void Animate (Vector3 collectedCoinPosition, int amount)
	{
        GameEvents.Instance.OnPropertyChange("Coin", amount);
        for (int i = 0; i < amount; i++) {
			//check if there's coins in the pool
			if (coinsQueue.Count > 0) {
				//extract a coin from the pool
				GameObject coin = coinsQueue.Dequeue ();
				RectTransform rect;
				rect = coin.GetComponent<RectTransform> ();
				coin.SetActive (true);
				//move coin to the collected coin pos
				coin.transform.position = collectedCoinPosition + new Vector3 (Random.Range (-spread, spread), Random.Range(-spread, spread), 0f);
				
				//animate coin to target position
				float duration = Random.Range (minAnimDuration, maxAnimDuration);
                targetPosition = TweenHelper.SwitchToRectTransform(target, rect);
                rect.DOAnchorPos(targetPosition, duration)
				.SetEase(easeType)
				.OnComplete(() =>
				{
					//executes whenever coin reach target position
					coin.SetActive(false);
					coinsQueue.Enqueue(coin);
					targetImage.PlayAnimation();
					GameManager.Instance.IncreaseCoin(1, false);
					
					Coins++;
				});
			}
		}
	}

	public void AddCoins (Vector3 collectedCoinPosition, int amount)
	{
		Animate (collectedCoinPosition, amount);
	}
}
