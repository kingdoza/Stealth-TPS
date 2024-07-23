using System;
using UnityEngine;

[Serializable]
public class Totem {
    [SerializeField] private Sprite image;
    [SerializeField] private string name;
    [SerializeField] [TextArea] private string info;
    private Sprite border;
    public string Name => name;
    public string Info => info;
    public Sprite Image => image;
    public TargetEnemy TargetEnemy { get; set; }
    [SerializeField] private EnemyVariables enemyVariables;
    public EnemyVariables EnemyVariables => enemyVariables;
    private bool isActivate = false;
    public bool Activation { get => isActivate;
        set {
            if(value == isActivate)
                return;
            isActivate = value;
            if(isActivate)
                TargetEnemy.ActiveTargetEffect();
            else
                TargetEnemy.DeactiveTargetEffect();
        }
    }

    public void Active() {
        TargetEnemy.ActiveTargetEffect();
    }

    public void Deactive() {
        TargetEnemy.DeactiveTargetEffect();
    }
}
