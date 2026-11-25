
using UnityEngine;
using System.Threading.Tasks;

#if !UNITY_SERVER
using BACKND.Database;
#endif

public class BackendDataBase
{
#if !UNITY_SERVER
    public Client DBClient;
#endif

    private bool isInitialized = false;
    private bool isInitializing = false;

    public async Task Setup()
    {
        if (isInitialized || isInitializing)
            return;

        isInitializing = true;

        try
        {
#if !UNITY_SERVER
            // 새 게임 DB 클라이언트 ID 입력 필요
            DBClient = new Client(string.Empty);

            await DBClient.Initialize();

            isInitialized = true;
            Debug.Log("데이터베이스 초기화 완료");
#else
            isInitialized = true;
            Debug.Log("[Server] BackendDataBase 초기화 생략");
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogError($"데이터베이스 초기화 실패: {e}");
        }
        finally
        {
            isInitializing = false;
        }
    }
}
