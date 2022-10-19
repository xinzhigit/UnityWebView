using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class Example : MonoBehaviour
{
    [SerializeField]
    private Camera camera;
    
    [SerializeField]
    private RectTransform anchorLeft;

    [SerializeField]
    private RectTransform anchorRight;

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
        
        HideBar();
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
        _webView.Init(
            cb: (msg) =>
            {
                Debug.Log($"CallFromJS[{msg}]");
                
                if (msg == "close") {
                    Destroy(_webView.gameObject);
                }
            },
            err: (msg) =>
            {
                Debug.Log($"CallOnError[{msg}]");
            },
            httpErr: (msg) =>
            {
                Debug.Log($"CallOnHttpError[{msg}]");
            },
            started: (msg) =>
            {
                Debug.Log($"CallOnStarted[{msg}]");
            },
            hooked: (msg) =>
            {
                Debug.Log($"CallOnHooked[{msg}]");
            },
            ld: (msg) =>
            {
                Debug.Log($"CallOnLoaded[{msg}]");
                
                // _webView.EvaluateJS(@"document.getElementById('items').remove();");
                
                _webView.EvaluateJS(@"Unity.call('ua=' + navigator.userAgent)");
                // _webView.EvaluateJS(
                //     "(function() {" +
                //     "  var button;" +
                //     "  button = document.createElement('button');" +
                //     "  button.style.position = 'fixed';" +
                //     "  button.style.right = '0px';" +
                //     "  button.style.top = '0px';" +
                //     "  button.style.zIndex = 2147483647;" +
                //     "  button.textContent = 'X';" +
                //     "  button.onclick = function() {" +
                //     "    Unity.call('close');" +
                //     "  };" +
                //     "  document.body.appendChild(button);" +
                //     "})()");
                
                // _webView.EvaluateJS(
                //     "(function() {" +
                //     "   var items = document.body;" +
                //     "   Unity.call('document body:' + items.tagName);" +
                //     "})()");
                //
                // _webView.EvaluateJS(
                //     "(function() {" +
                //     "   var items = document.activeElement;" +
                //     "   Unity.call('active element:' + items.tagName);" +
                //     "})()");
            }
        );

        var leftAnchor = RectTransformUtility.WorldToScreenPoint(camera, anchorLeft.transform.position);
        var rightAnchor = RectTransformUtility.WorldToScreenPoint(camera, anchorRight.transform.position);
        
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

        HideBar();
    }

    private void HideBar()
    {
        if (_webViewGO == null)
        {
            return;
        }
        
        _webView.EvaluateJS(
            "(function() {" +
            "   var items = document.getElementsByTagName('ytm-pivot-bar-renderer');" +
            "	if(items.length === 0)" + 
            "		return;" +
            "   var bar = items[0];" +
            "   bar.remove();" +
            "   Unity.call('remove element:' + bar.className);" +
            "})()");
    }
}
