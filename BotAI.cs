using TicTacGame;
using UnityEngine;

public class BotAI : MonoBehaviour
{
   private int allExpirienceDificultCoef;
   private int allMatchDificultCoef;
   private int botsMatchTactic;

   private CellController currentToBuildTarget;
   public CellController CurrentToBuildTarget { get => currentToBuildTarget; set => currentToBuildTarget = value; }

   private void Awake()
   {
      allExpirienceDificultCoef = 15 * SaveSystem.sv.dificult;
      if (allExpirienceDificultCoef > 150) allExpirienceDificultCoef = 150; // just limit
   }

   public void EnemyMove()
   {
      if (CurrentToBuildTarget == null)
      {
         RandomMove();
         return;
      }

      CellController targetToWin = null;
      CellController targetDanger = null;
      CellController targetToBuild = null;

      // Check on win cell
      int[] countToWin = CurrentToBuildTarget.GetCountToWin();
      if (countToWin[botsMatchTactic] == GameManager.inst.countForWin - 1)
      {
         targetToWin = CurrentToBuildTarget.GetCellByTactic(botsMatchTactic);
      }
      // Check on danger cell
      for (int i = 0; i < PlayingField.inst.cells.Count; i++)
      {
         if (PlayingField.inst.cells[i].state != GameManager.inst.playerRole) continue;

         targetDanger = PlayingField.inst.cells[i].GetDangerCellForEnemy();
         if (targetDanger != null) break; // break if we searched
      }
      // Check on build way
      targetToBuild = CurrentToBuildTarget.GetCellByTactic(botsMatchTactic);
      if (targetToBuild == null)
      {
         // change bots tactic
         for (int i = 0; i < 4; i++)
         {
            targetToBuild = CurrentToBuildTarget.GetCellByTactic(i);
            if (targetToBuild != null)
            {
               botsMatchTactic = i;
               break;
            }
         }
      }

      // Do something inteligent
      if (targetToWin != null && DoOrNo())
      {
         targetToWin.ChangeState(false);
         return;
      }
      if (targetDanger != null && DoOrNo())
      {
         targetDanger.ChangeState(false);
         return;
      }
      if (targetToBuild != null && DoOrNo())
      {
         targetToBuild.ChangeState(false);
         return;
      }
      RandomMove();
   }

   private void RandomMove()
   {
      // searching empty cell
      int random = Random.Range(0, PlayingField.inst.cells.Count);
      while (PlayingField.inst.cells[random].state != State.None)
      {
         random = Random.Range(0, PlayingField.inst.cells.Count);
      }
      // taking this empty cell for bot
      CurrentToBuildTarget = PlayingField.inst.cells[random];
      CurrentToBuildTarget.ChangeState(false);
      botsMatchTactic = Random.Range(0, 4);
   }

   private bool DoOrNo()
   {
      if (allMatchDificultCoef > 30) allMatchDificultCoef = 30; // just limit

      bool result = (Random.Range(0f + allMatchDificultCoef + allExpirienceDificultCoef * 0.7f, allExpirienceDificultCoef) > Random.Range(0, 100));
      if (result) allMatchDificultCoef -= 5;
      else allMatchDificultCoef += 5;
      return result;
   }
}