using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using BackEnd;
using LitJson;

public enum LoginType
{
    None = -1,
    Google = 0,
    Facebook,
    Apple,
    Steam,

    Guest = 10,
}

public class BackendLogin
{
    LoginType loginType = LoginType.None;
    public LoginType LoginType { get { return loginType; } set { loginType = value; } }

    // success, errMsg, token
    public Action<bool, string, string> OnSignInWithApple = null;

    // 테스트: 입력한 ID로 로그인, 없으면 가입 (비밀번호도 ID)
    public void GuestLogin(string _uniqueIdentifier, Action<BackendReturnObject> _OnComplete = null)
    {
        CustomLogin(_uniqueIdentifier, _uniqueIdentifier, callback =>
        {
            if (callback.IsSuccess())
            {
                LoginType = LoginType.Guest;
                PlayerPrefs.SetInt("LoginType", (int)LoginType);
                Debug.Log("로그인이 성공했습니다. : " + callback);
            }
            else
            {
                Debug.LogError("로그인이 실패했습니다. : " + callback);
                if (callback.GetStatusCode().Equals("401"))
                {
                    CustomSignUp(_uniqueIdentifier, _uniqueIdentifier, (signCallback)=>
                    {
                        if (signCallback.IsSuccess())
                        {
                            GuestLogin(_uniqueIdentifier, _OnComplete);
                        }
                    });
                    return;
                }
            }
            if (_OnComplete != null)
                _OnComplete(callback);
        });
    }

    public void CustomSignUp(string id, string pw, Action<BackendReturnObject> onComplete = null)
    {
        Debug.Log("회원가입을 요청합니다.");
        Backend.BMember.CustomSignUp(id, pw, callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("회원가입에 성공했습니다. : " + callback);
            }
            else
            {
                Debug.LogError("회원가입에 실패했습니다. : " + callback);
            }
            if (onComplete != null)
                onComplete(callback);
        });
    }

    public void CustomLogin(string id, string pw, Action<BackendReturnObject> onComplete = null)
    {
        Debug.Log("로그인을 요청합니다.");
        Backend.BMember.CustomLogin(id, pw, callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("로그인이 성공했습니다. : " + callback);
            }
            else
            {
                Debug.LogError("로그인이 실패했습니다. : " + callback);
            }
            if (onComplete != null)
                onComplete(callback);
        });
    }

    public void UpdateNickname(string nickname, Action<BackendReturnObject, string> onComplete = null)
    {
        Debug.Log("닉네임 변경을 요청합니다.");
        Backend.BMember.UpdateNickname(nickname, callback=>
        {
            if (callback.IsSuccess())
            {
                // Debug.Log("닉네임 변경에 성공했습니다 : " + callback);
            }
            else
            {
                // Debug.LogError("닉네임 변경에 실패했습니다 : " + callback);
            }
            onComplete?.Invoke(callback, nickname);
        });
    }

    public void GetUserInfo(Action<BackendReturnObject> _complete = null)
    {
        Backend.BMember.GetUserInfo(callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("유저정보 불러오기 성공");
            }

            if (_complete != null)
                _complete(callback);
        });
    }


    public void TokenLogin(Action<BackendReturnObject> _complete = null)
    {
        Backend.BMember.LoginWithTheBackendToken(callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("자동 로그인에 성공했습니다");
            }

            if (_complete != null)
                _complete(callback);
        });
    }

    public void GPGSLogin(string _token, Action<BackendReturnObject> _OnComplete = null)
    {
        Debug.Log("GPGSLogin.AuthorizeFederation : " + _token);
        Backend.BMember.AuthorizeFederation(_token, FederationType.Google, "gpgs", callback =>
        {
            if (callback.IsSuccess())
            {
                LoginType = LoginType.Google;
                PlayerPrefs.SetInt("LoginType", (int)LoginType);
            }
            Debug.Log("AuthorizeFederation : " + callback);

            if (_OnComplete != null)
                _OnComplete(callback);
            _OnComplete = null;
        });
    }
    public void AppleLogin(string _token, Action<BackendReturnObject> _OnComplete = null)
    {
        Debug.Log("AppleLogin.AuthorizeFederation : " + _token);
        Backend.BMember.AuthorizeFederation(_token, FederationType.Apple, "ios", callback =>
        {
            if (callback.IsSuccess())
            {
                LoginType = LoginType.Apple;
                PlayerPrefs.SetInt("LoginType", (int)LoginType);
            }
            Debug.Log("AuthorizeFederation : " + callback);

            if (_OnComplete != null)
                _OnComplete(callback);
            _OnComplete = null;
        });
    }


    public void ChangeCustomToFederation(string _token, FederationType _federationType, Action<BackendReturnObject> _onComplete = null)
    {
        Backend.BMember.ChangeCustomToFederation(_token, _federationType, callback =>
        {
            if (callback.IsSuccess())
            {
                if (_federationType == FederationType.Google)
                    loginType = LoginType.Google;
                else if (_federationType == FederationType.Apple)
                    loginType = LoginType.Apple;

                PlayerPrefs.SetInt("LoginType", (int)LoginType);
            }
            if (_onComplete != null)
                _onComplete(callback);
        });
    }

    public void Logout(Action<BackendReturnObject> _OnComplete = null)
    {
        Backend.BMember.Logout(callback =>
        {
            if (callback.IsSuccess())
            {
                loginType = LoginType.None;
                PlayerPrefs.DeleteKey("LoginType");
            }

            if (_OnComplete != null)
                _OnComplete(callback);
        });
    }
    public void WithdrawAccount(int graceHours = 0, Action<BackendReturnObject> _OnComplete = null)
    {
        Backend.BMember.WithdrawAccount(graceHours, callback =>
        {
            if (callback.IsSuccess())
            {
                loginType = LoginType.None;
                PlayerPrefs.DeleteKey("LoginType");
            }

            if (_OnComplete != null)
                _OnComplete(callback);
        });
    }
    public void RevokeAppleToken(string authorizationCode, Action<BackendReturnObject, string> onComplete = null)
    {
        if (string.IsNullOrEmpty(authorizationCode))
        {
            onComplete?.Invoke(null, authorizationCode);
            return;
        }

        Backend.BMember.RevokeAppleToken(authorizationCode, callback =>
        {
            if (callback.IsSuccess())
            {

            }
            else
            {

            }
            onComplete?.Invoke(callback, authorizationCode);
        });
    }

    public void CheckNicknameDuplication(string _nickname, Action<BackendReturnObject> _onComplete = null)
    {
        Backend.BMember.CheckNicknameDuplication(_nickname, callback =>
        {
            if (_onComplete != null)
                _onComplete(callback);
        });
    }

    public void SignInWithApple()
    {
        TheBackend.ToolKit.AppleLogin.Android.OnCloseButtonClick = () =>
        {
            Debug.Log("창이 닫혔습니다");
            HandleSignInWithAppleComplete(false, "Close Window", null);
        };

        TheBackend.ToolKit.AppleLogin.Android.AppleLogin("com.threewavestudio.dragonidlerpg.backend", out var error, true, token =>
        {
            Debug.Log("토큰 : " + token);
            Debug.Log("토큰 발급이 완료되었습니다. 로그인이 가능합니다.");

            // 경고! : 다음과 같이 동기 함수를 호출하지 마세요
            // var bro = Backend.BMember.AuthorizeFederation(token, FederationType.Apple);

            // 아래와 같이 비동기 함수를 호출해주세요,
            // AppleLogin(token, _OnComplete);
            HandleSignInWithAppleComplete(true, null, token);
        });

        if (string.IsNullOrEmpty(error) == false)
        {
            Debug.Log("에러 : " + error);
            HandleSignInWithAppleComplete(false, error, null);
        }
    }
    void HandleSignInWithAppleComplete(bool _isSuccess, string _errorMessage, string _token)
    {
        // Debug.Log(string.Format("HandleGoogleSignInComplete\n isSuccess : {0} _errorMessage : {1} _token : {2} ", _isSuccess, _errorMessage, _token));
        // (bool isSuccess, string errorMessage, string token)
        OnSignInWithApple?.Invoke(_isSuccess, _errorMessage, _token);
    }
}
