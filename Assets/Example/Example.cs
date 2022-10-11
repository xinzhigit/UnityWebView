using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class Example : MonoBehaviour
{
    [SerializeField]
    private Camera camera;
    
    [SerializeField]
    private Image imageLeft;

    [SerializeField]
    private Image imageRight;

    [SerializeField]
    private Button btnOpen;
    
    [SerializeField]
    private Button btnClose;
    
    [SerializeField]
    private Button btnBack;
    
    [SerializeField]
    private Button btnForward;
    
    [SerializeField]
    private string url;

    private GameObject _webViewGO;

    private WebViewObject _webView;
    
    private void Start()
    {
        btnOpen.onClick.AddListener(OnOpen);
        btnClose.onClick.AddListener(OnClose);
        
        btnBack.onClick.AddListener(OnBack);
        btnForward.onClick.AddListener(OnForward);
        
        btnBack.gameObject.SetActive(false);
        btnForward.gameObject.SetActive(false);
    }

    private void Update()
    {
        CheckBackForward();
    }

    private void OnOpen()
    {
        _webViewGO = new GameObject("WebView")
        {
            transform =
            {
                parent = transform
            }
        };

        _webView = _webViewGO.AddComponent<WebViewObject>();
        _webView.Init();

        var leftAnchor = RectTransformUtility.WorldToScreenPoint(camera, imageLeft.transform.position);
        var rightAnchor = RectTransformUtility.WorldToScreenPoint(camera, imageRight.transform.position);
        
        var left = (int) leftAnchor.x;
        var top = Screen.height - (int)leftAnchor.y;
        var right = Screen.width - (int) rightAnchor.x;
        var bottom = (int) rightAnchor.y;
        
        _webView.SetMargins(left, top, right, bottom);
        _webView.SetTextZoom(100);
        _webView.SetVisibility(true);
        
        _webView.LoadURL(url);
        
        Debug.Log($"Set web view {leftAnchor} {rightAnchor} {left} {top} {right} {bottom}");
    }

    private void OnClose()
    {
        if (_webViewGO == null)
        {
            return;
        }
        
        Object.Destroy(_webViewGO);
    }
    
    private void OnBack()
    {
        if (_webViewGO == null)
        {
            return;
        }
        
        _webView.GoBack();
    }
    
    private void OnForward()
    {
        if (_webViewGO == null)
        {
            return;
        }

        _webView.GoForward();
    }

    private void CheckBackForward()
    {
        if (_webView is null)
        {
            return;
        }
        
        btnBack.gameObject.SetActive(_webView.CanGoBack());
        btnForward.gameObject.SetActive(_webView.CanGoForward());
    }
}
