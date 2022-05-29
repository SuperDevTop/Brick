using UnityEngine;
using UnityEngine.UI;

public class UINavigation : MonoBehaviour
{
    public GameObject tabDigitalGoodUI;
    public GameObject tabVirtualUI;
    public GameObject tabSubscriptionUI;

    public Button buttonDigital;
    public Button buttonVirtual;
    public Button buttonSubscription;

	public GameObject ShoppingCartUI;

    void Start()
    {
		if(tabDigitalGoodUI.activeSelf) {
        	buttonDigital.interactable = false;
		}
		if(tabVirtualUI.activeSelf) {
			buttonVirtual.interactable = false;
		}
		if(tabSubscriptionUI.activeSelf) {
			buttonSubscription.interactable = false;
		}
    }

    public void SwitchTab(Button pSender)
    {
        ResetTabIndicator();
		pSender.interactable = false;
        switch (pSender.gameObject.name)
        {
            case "Nav DigitalGoods":
                tabDigitalGoodUI.SetActive(true);
				buttonDigital.interactable = false;
                break;
            case "Nav VirtualCurrency":
                tabVirtualUI.SetActive(true);
                break;
            case "Nav Subscription":
                tabSubscriptionUI.SetActive(true);
                break;
			case "Nav OutOfShard":
				tabVirtualUI.SetActive(true);
				pSender.interactable = true;
				buttonVirtual.interactable = false;
				break;
			case "Button Title":
				tabDigitalGoodUI.SetActive(true);
				buttonDigital.interactable = false;
				pSender.interactable = true;
				break;
            default: break;
        }

		if(ShoppingCartUI.activeSelf){
			ShoppingCartUI.SetActive(false);
		}
    }

    private void ResetTabIndicator()
    {
        tabDigitalGoodUI.SetActive(false);
        tabVirtualUI.SetActive(false);
        tabSubscriptionUI.SetActive(false);

        buttonDigital.interactable = true;
        buttonVirtual.interactable = true;
        buttonSubscription.interactable = true;
    }
}
