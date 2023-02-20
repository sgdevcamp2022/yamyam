using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class Auth : MonoBehaviour
{

    static string _authAccessTokenUrl = "http://127.0.0.1:8000/accounts/check_access_token/";
    static string _authRefreshTokenUrl = "http://127.0.0.1:8000/accounts/check_refresh_token/";
    static HttpResponseMessage _response;


    public static async Task<bool> AuthToken()
    {
        Debug.Log("AuthToken 인증중");
        HttpClient _httpClient = new HttpClient();

        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Access-Token", Crypto.AESDecrypt128(CryptoType.AccessToken));
        _response = await _httpClient.GetAsync(_authAccessTokenUrl);

        switch ((int)_response.StatusCode)
        {
            case 200:
                Debug.Log("AccessToken 인증성공!");
                return true;
            case 401:
                Debug.Log("AccessToken 인증실패 ㅠ");
                return await AuthRefreshToken();
            default:
                
                Debug.Log("AccessToken 401말고 다른 오류로 인한 인증실패 ㅠ");
                Debug.Log("ERRORCODE : " + (int)_response.StatusCode);
                Debug.Log("RESPONSE : " + _response);
                return await AuthRefreshToken();
        }
        /*
         * AccessToken 확인
         * _AccessTokenStatus
         * switch(_AccessTokenStatus)
         * {
         * case 200:
         * return true;
         * break;
         * 
         * case 401:
         * return uthRefreshToken();
         * break;
         * 
         * default:
         * return false;
         * }
         */
    }


    public static async Task<bool> AuthRefreshToken()
    {
        HttpClient _httpClient = new HttpClient();

        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Refresh-Token", Crypto.AESDecrypt128(CryptoType.RefreshToken));
        _response = await _httpClient.GetAsync(_authRefreshTokenUrl);


        switch ((int)_response.StatusCode)
        {
            case 200:
                Debug.Log("RefreshToken 인증성공!");
                SetTokens();
                return true;
            case 401:
                Debug.Log("RefreshToken 인증실패!");
                return false;
            default:
                Debug.Log("RefreshToken 401말고 다른 오류로 인한 인증실패 ㅠ");
                Debug.Log("ERRORCODE : " + (int)_response.StatusCode);
                Debug.Log("RESPONSE : " + _response);
                return false;
        }
        /*
         * RefreshToken 확인
         * _RefreshTokenStatus
         * switch(_RefreshTokenStatus)
         * {
         * case 200:
         * AccessToken과 RefreshToken다시받기.
         * Crypto.AESEncrypt128(_AccessToken, CryptoType.AccessToken);
         Crypto.AESEncrypt128(_RefreshToken, CryptoType.RefreshToken);
         * return true;
         * break;
         * 
         * case 401:
         * return false;
         * break;
         * 
         * }
         */
    }

    public static void SetTokens()
    {
        foreach (var token in _response.Headers.GetValues("Access-Token"))
        {
            Crypto.AESEncrypt128(token, CryptoType.AccessToken);
        }
        foreach (var token in _response.Headers.GetValues("Refresh-Token"))
        {
            Crypto.AESEncrypt128(token, CryptoType.AccessToken);
        }

    }
}
