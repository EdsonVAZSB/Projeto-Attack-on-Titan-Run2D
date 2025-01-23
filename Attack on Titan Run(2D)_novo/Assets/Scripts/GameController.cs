using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject groundPrefab; // Prefab do segmento de chão
    public GameObject enemyPrefab; // Prefab do inimigo
    public Transform enemyParent;
    GameObject enemy;
    public int initialGroundCount = 3; // Quantidade inicial de segmentos de chão
    private int maxGroundSegments = 3;
    public float groundSize = 10f; // Tamanho de cada segmento de chão
    private int count; 
    public bool playing;
    public static GameController gameController;
    private float timeElapsed;
    private float xValue;
    private List<GameObject> groundSegments = new List<GameObject>(); // Lista para armazenar segmentos de chão
    private Transform playerTransform; // Referência ao jogador ou à câmera

    void Awake(){
        gameController = this;
    }

    void Start()
    {
        playing = false;
        // Encontra o jogador pela tag "Player"
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // Inicializa o chão preenchendo uma quantidade inicial de segmentos
        InitializeGround();

        count = 0;
    }

    private void InitializeGround()
    {
        // Cria segmentos de chão iniciais para preencher a tela
        for (int i = 0; i < initialGroundCount; i++)
        {
            Vector3 spawnPosition = new Vector3(i * groundSize, -1, 0);
            SpawnGroundSegment(spawnPosition);
        }
    }

    private void SpawnGroundSegment(Vector3 position)
    {
        // Instancia um novo segmento de chão e o adiciona à lista
        GameObject newGround = Instantiate(groundPrefab, position, Quaternion.identity);
        groundSegments.Add(newGround);
    }

    void Update()
    {
        if(playing){
            InstantiateGround();
            InstantiateEnemy(); 
        }
         
    }

    void InstantiateGround(){
        // Verifica se o jogador está perto das extremidades do chão e adiciona novos segmentos conforme a direção
        if (playerTransform.position.x < groundSegments[0].transform.position.x + groundSize / 2)
        {
            // Jogador está indo para a esquerda
            // Cria um novo segmento à esquerda
            GameObject newGround = Instantiate(
                groundPrefab,
                new Vector3(
                    groundSegments[0].transform.position.x - groundSize,
                    groundSegments[0].transform.position.y,
                    groundSegments[0].transform.position.z
                ),
                Quaternion.identity
            );

            // Adiciona o novo segmento no início da lista
            groundSegments.Insert(0, newGround);

            // Remove o último segmento se necessário
            if (groundSegments.Count > maxGroundSegments)
            {
                Destroy(groundSegments[groundSegments.Count - 1]);
                groundSegments.RemoveAt(groundSegments.Count - 1);
            }
        }
        else if (playerTransform.position.x > groundSegments[groundSegments.Count - 1].transform.position.x - groundSize / 2)
        {
            // Jogador está indo para a direita
            // Cria um novo segmento à direita
            GameObject newGround = Instantiate(
                groundPrefab,
                new Vector3(
                    groundSegments[groundSegments.Count - 1].transform.position.x + groundSize,
                    groundSegments[groundSegments.Count - 1].transform.position.y,
                    groundSegments[groundSegments.Count - 1].transform.position.z
                ),
                Quaternion.identity
            );

            // Adiciona o novo segmento no final da lista
            groundSegments.Add(newGround);

            // Remove o primeiro segmento se necessário
            if (groundSegments.Count > maxGroundSegments)
            {
                Destroy(groundSegments[0]);
                groundSegments.RemoveAt(0);
            }
        }
    }

    public void SetPlaying(bool val){
        playing = val;
        if(!playing){
            Time.timeScale = 0;
        }
    }

    void InstantiateEnemy(){
        timeElapsed += Time.deltaTime;
        if(timeElapsed > 3){
            if(count < 10){
                count++;
                timeElapsed = 0;
                enemy = Instantiate(enemyPrefab, new Vector2(xValue,-1), Quaternion.identity);
                xValue += 3;
                enemy.transform.SetParent(enemyParent, false);
                StartCoroutine(WaittoDeactivateEnemy(enemy));
            }else{
                enemy = GetEnemyAvailable(enemyParent);
                enemy.transform.position = new Vector2(xValue,-1);
                enemy.SetActive(true);
                StartCoroutine(WaittoDeactivateEnemy(enemy));
            }
            
        }
    }

    GameObject GetEnemyAvailable(Transform parent){
        for(int i=0; i<parent.childCount; i++){
            if(!parent.GetChild(i).gameObject.activeSelf){
                return parent.GetChild(i).gameObject;
            }
        }
        return parent.GetChild(0).gameObject;
    }

     IEnumerator WaittoDeactivateEnemy(GameObject enemy)
    {
        yield return new WaitForSecondsRealtime(10);
        enemy.SetActive(false);

    }
}
