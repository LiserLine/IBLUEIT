using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stars : MonoBehaviour {
    
    //Conta quantas estrelas o jogador alcançou.
    public int score;

    //Ajusta os sprites de estrela vazia.
    public Sprite[] star = new Sprite[3];
	public Sprite starOn;

    [SerializeField]
    public ScoreMenu scoreMenu;

    //Ajusta a velocidade de preenchimento.
    [SerializeField]
    private float lerpSpeed;

    //Quantidade de preenchimento.

    //Define a imagem de preenchimento.
    [SerializeField]
    private Image[] content = new Image[3];

    public void FillStars(int index)
    {
        content[index].fillAmount = 1;

    }
    public void UnfillStars()
    {
        content[0].fillAmount = 0;
        content[1].fillAmount = 0;
        content[2].fillAmount = 0;
    }

    public void FillStarsFinal(int starQty)
    {
        for (int i = 0; i < starQty; i++)
        {
            content[i].fillAmount = 1;
        }

    }

    public void OnFinish()
    {
        scoreMenu.ToggleScoreMenu();
    }
}
