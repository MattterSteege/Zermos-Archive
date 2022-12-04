using UI.Views;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class KoppelingenSubView : SubView
    {
        [Header("Koppelingen")]
        [SerializeField] private Button SomtodayKoppeling;
        [SerializeField] private Image Somtodaygekoppeld;
        [SerializeField] private Student student;
        [SerializeField] private Button ZermeloKoppeling;
        [SerializeField] private Image Zermelogekoppeld;
        [SerializeField] private User user;
        [SerializeField] private Button InfowijsKoppeling;
        [SerializeField] private Image Infowijsgekoppeld;
        [SerializeField] private SessionAuthenticatorInfowijs sessionAuthenticatorInfowijs;
    
        public override void Initialize()
        {
            CheckKoppelingen();
            
            SomtodayKoppeling.onClick.AddListener(() =>
            {
                ViewManager.Instance.ShowNewView<ConnectSomtodayView>();
            });
    
            ZermeloKoppeling.onClick.AddListener(() =>
            {
                ViewManager.Instance.ShowNewView<ConnectZermeloView>();
            });
        
            InfowijsKoppeling.onClick.AddListener(() =>
            {
                ViewManager.Instance.ShowNewView<ConnectInfowijsView>();
            });
        
            base.Initialize();
        }

        private void CheckKoppelingen()
        {
            bool SomtodayIslinked = PlayerPrefs.GetString("somtoday-access_token") != "";
            bool ZermeloIslinked = PlayerPrefs.GetString("zermelo-access_token") != "";
            bool InfowijsIslinked = PlayerPrefs.GetString("infowijs-access_token", "") != "";

            if (SomtodayIslinked)
            {
                Somtodaygekoppeld.color = new Color(0.172549f, 0.9333333f, 0.5568628f);
            }
            else
            {
                Somtodaygekoppeld.color = new Color(0.9921569f, 0.4509804f, 0.4431373f);
            }
        
            if (ZermeloIslinked)
            {
                Zermelogekoppeld.color = new Color(0.172549f, 0.9333333f, 0.5568628f);
            }
            else
            {
                Zermelogekoppeld.color = new Color(0.9921569f, 0.4509804f, 0.4431373f);
            }
        
            if (InfowijsIslinked)
            {
                Infowijsgekoppeld.color = new Color(0.172549f, 0.9333333f, 0.5568628f);
            }
            else
            {
                Infowijsgekoppeld.color = new Color(0.9921569f, 0.4509804f, 0.4431373f);
            }
        }
    }
}
