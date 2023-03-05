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
        [SerializeField] private Button ZermeloKoppeling;
        [SerializeField] private Image Zermelogekoppeld;
        [SerializeField] private Button InfowijsKoppeling;
        [SerializeField] private Image Infowijsgekoppeld;

        public override void Initialize()
        {
            CheckKoppelingen();
            
            SomtodayKoppeling.onClick.AddListener(() =>
            {
                SwitchView.Instance.Show<ConnectSomtodayView>();
            });
    
            ZermeloKoppeling.onClick.AddListener(() =>
            {
                SwitchView.Instance.Show<ConnectZermeloView>();
            });
        
            InfowijsKoppeling.onClick.AddListener(() =>
            {
                SwitchView.Instance.Show<ConnectInfowijsView>();
            });
        
            base.Initialize();
        }

        public override void Refresh(object args)
        {
            SomtodayKoppeling.onClick.RemoveAllListeners();
            ZermeloKoppeling.onClick.RemoveAllListeners();
            InfowijsKoppeling.onClick.RemoveAllListeners();
            base.Refresh(args);
        }

        private void CheckKoppelingen()
        {
            bool SomtodayIslinked = LocalPrefs.GetString("somtoday-access_token") != null;
            bool ZermeloIslinked = LocalPrefs.GetString("zermelo-access_token") != null;
            bool InfowijsIslinked = LocalPrefs.GetString("infowijs-access_token") != null;

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
