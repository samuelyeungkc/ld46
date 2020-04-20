﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultsScreen : MonoBehaviour {
    [SerializeField] SessionData sessionData;
    [SerializeField] Text peopleDied;
    [SerializeField] Text peopleSave;
    [SerializeField] Text thingsIgnited;
    [SerializeField] Text thingsExtinguished;
    [SerializeField] Text waterUsed;
    [SerializeField] Text summary;

    void Start() {
        peopleDied.text = sessionData.peopleDied.ToString();
        peopleSave.text = sessionData.peopleSaved.ToString();
        thingsIgnited.text = sessionData.unitsBurned.ToString();
        thingsExtinguished.text = sessionData.unitsExtinguished.ToString();
        waterUsed.text = sessionData.waterUsed.ToString() + " L";

        SessionData.Ratings r = sessionData.GetRating();
        SessionData.Title t = sessionData.GetTitle();

        summary.text = "Fire truck unit LDXLVI was immediately dispatched to the scene of the fire";

        switch (r.finalRating) {
            case SessionData.Rating.Bad:
                summary.text += ", only to <color=#DF3E23>further worsen the situation.</color> ";
                break;
            case SessionData.Rating.Average:
                summary.text += ". ";
                break;
            case SessionData.Rating.Good:
                summary.text += ", and <color=#DF3E23>handled the situation to the best of their ability.</color> ";
                break;
            case SessionData.Rating.Perfect:
                summary.text += ", and <color=#DF3E23>valiantly handled the situation.</color> ";
                break;
        }

        switch (r.peopleRating) {
            case SessionData.Rating.Bad:
                summary.text += "Unfortunately, there were <color=#DF3E23>unprecendented casualties</color>";
                break;
            case SessionData.Rating.Average:
                summary.text += "Unfortunately, there were <color=#DF3E23>many casualties</color>";
                break;
            case SessionData.Rating.Good:
                summary.text += "Unfortunately, There were <color=#DF3E23>a few casualties</color>";
                break;
            case SessionData.Rating.Perfect:
                summary.text += "There were <color=#DF3E23>very few casualties</color>";
                break;
        }

        switch (r.unitRating) {
            case SessionData.Rating.Bad:
                summary.text += " and <color=#DF3E23>countless damaged buildings.</color>";
                break;
            case SessionData.Rating.Average:
                summary.text += " and they did their best to extinguish buildings, <color=#DF3E23>although some did burn down.</color>";
                break;
            case SessionData.Rating.Good:
                summary.text += " and <color=#DF3E23>most buildings</color> were put out within time.";
                break;
            case SessionData.Rating.Perfect:
                summary.text += " and <color=#DF3E23>almost no building</color> went unrescued.";
                break;
        }

        switch (t) {
            case SessionData.Title.None:
                break;
            case SessionData.Title.GrimReaper:
                summary.text += " Your negligence for civilian life has been noted.";
                break;
            case SessionData.Title.Arsonist:
                summary.text += " Witnesses state hardly any attempt to extinguish buildings were made.";
                break;
            case SessionData.Title.HydroHomie:
                summary.text += " Environmentalists are concerned about the excessive use of water.";
                break;
        }
    }
}
