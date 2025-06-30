using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public GameObject ficVermelha;
    public GameObject ficAmarela;
    public int[,] board = new int[7, 6]; // 0 = vazio, 1 = vermelho, 2 = amarelo
    private float tamFicha = 1f;

    private int currentPlayer = 1;
    private bool acabou = false;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        infoText.text = "Jogador Vermelho começa!";
        botao.onClick.AddListener(Reinicio);
        telaVitória.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (acabou) return;
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int coluna = Mathf.RoundToInt(worldPos.x);

            if (coluna >= 0 && coluna < 7)
            {
                PosicFicha(coluna);
            }
        }
    }

    void PosicFicha(int coluna)
    {
        for (int linha = 5; linha >= 0; linha--)
        {
            if (board[coluna, linha] == 0)
            {
                board[coluna, linha] = currentPlayer;

                GameObject prefab = currentPlayer == 1 ? ficVermelha : ficAmarela;
                Vector3 spawnPos = new Vector3(coluna * tamFicha, 6f, 0);
                Vector3 targetPos = new Vector3(coluna * tamFicha, -linha * tamFicha, 0);

                GameObject disc = Instantiate(prefab, spawnPos, Quaternion.identity);
                StartCoroutine(anim(disc, targetPos)); ;

                if (SeraseVenceu(coluna, linha))
                {
                    infoText.text = "";
                    telaVitória.SetActive(true);
                    vitoriaText.text = "Jogador " + (currentPlayer == 1 ? "Vermelho" : "Amarelo") + " venceu!";
                    acabou = true;
                }
                

                currentPlayer = 3 - currentPlayer; // Alterna entre 1 e 2
                return;
            }
        }
    }


    bool SeraseVenceu(int col, int linha)
    {
        int player = board[col, linha];

        // horiz, vert, diag/
        if (CountInDirection(col, linha, 1, 0, player) + CountInDirection(col, linha, -1, 0, player) >= 3) return true;
        if (CountInDirection(col, linha, 0, 1, player) + CountInDirection(col, linha, 0, -1, player) >= 3) return true;
        if (CountInDirection(col, linha, 1, 1, player) + CountInDirection(col, linha, -1, -1, player) >= 3) return true;
        if (CountInDirection(col, linha, 1, -1, player) + CountInDirection(col, linha, -1, 1, player) >= 3) return true;

        return false;
    }

    int CountInDirection(int startCol, int startLinha, int dirCol, int dirLinha, int player)
    {
        int count = 0;
        int col = startCol + dirCol;
        int linha = startLinha + dirLinha;

        while (col >= 0 && col < 7 && linha >= 0 && linha < 6 && board[col, linha] == player)
        {
            count++;
            col += dirCol;
            linha += dirLinha;
        }

        return count;
    }

    System.Collections.IEnumerator anim(GameObject disc, Vector3 target)
    {
        float speed = 15f;

        while (Vector3.Distance(disc.transform.position, target) > 0.01f)
        {
            disc.transform.position = Vector3.MoveTowards(disc.transform.position, target, speed * Time.deltaTime);
            yield return null;
        }

        disc.transform.position = target;
    }

    //------------------ SCRIPT da UI -----------------------------/

    public TextMeshProUGUI infoText;
    public GameObject telaVitória;
    public TextMeshProUGUI vitoriaText;
    public Button botao;


    void Reinicio()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

