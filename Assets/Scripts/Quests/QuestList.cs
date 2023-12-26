﻿using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Saving;
using UnityEngine;


namespace RPG.Quests
{

  public class QuestList : MonoBehaviour, ISaveable
  {
    private List<QuestStatus> statuses = new List<QuestStatus>();

    public event Action onUpdateQuest;


    public IEnumerable<QuestStatus> GetStatues()
    {
      return statuses;
    }

    public void AddQuest(Quest quest)
    {
      if (HasQuest(quest))
      {
        return;
      }

      QuestStatus newStatus = new QuestStatus(quest);

      statuses.Add(newStatus);
      if (onUpdateQuest != null)
      {
        onUpdateQuest();
      }
    }

    private bool HasQuest(Quest quest)
    {
      return GetQuestStatus(quest) != null;
    }

    private QuestStatus GetQuestStatus(Quest quest)
    {
      foreach (QuestStatus status in statuses)
      {
        if (status.GetQuest() == quest)
        {
          return status;
        }
      }
      return null;

    }

    public void CompleteObjective(Quest quest, string objective)
    {
      QuestStatus status = GetQuestStatus(quest);
      if (status != null)
      {
        status.CompleteObjective(objective);
        if (onUpdateQuest != null)
        {
          onUpdateQuest();
        }
      }
    }

    public object CaptureState()
    {
      List<object> state = new List<object>();
      foreach (QuestStatus status in statuses)
      {
        state.Add(status.CaptureState());
      }
      return state;
    }

    public void RestoreState(object state)
    {
      List<object> stateList = state as List<object>;

      statuses.Clear();
      if (stateList != null)
      {
        foreach (object stateObject in stateList)
        {
          statuses.Add(new QuestStatus(stateObject));
        }
      }



      if (onUpdateQuest != null)
      {
        onUpdateQuest();
      }
    }
  }
}
