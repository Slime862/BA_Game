using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ba
{
    public class HealthBarUI : MonoBehaviour
    {
        public GameObject healthBarHolderPrefab;
        public Transform barPoint;
        private Image healthSilder;
        private Transform UIbar;
        public bool alwaysVisible;
        public float visibleTime;
        private float timeLeft;
        private Transform camera;

        public Canvas canvas;
        private void OnEnable()
        {
            camera = Camera.main.transform;
            canvas = GameObject.Find("HealthBarCanvas").GetComponent<Canvas>();
            UIbar = Instantiate(healthBarHolderPrefab, canvas.transform).transform;
            healthSilder = UIbar.GetChild(0).GetComponent<Image>();
            UIbar.gameObject.SetActive(alwaysVisible);
        }
        public void UpdateHealthBar(int currentHealth, int maxHealth)
        {
            if (UIbar == null)
            {
                return;
            }
            if (currentHealth <= 0)
            {
                Destroy(UIbar.gameObject);
                return;
            }
            if (UIbar.gameObject != null)
            {
                UIbar.gameObject.SetActive(true);//每次攻击必须让敌人血条强制可见
                timeLeft = visibleTime;
                float sliderPercent = (float)currentHealth / maxHealth;
                healthSilder.fillAmount = sliderPercent;
            }
        }
        private void LateUpdate()//在人物移动后再保证血条跟着移动
        {
            if (UIbar == null)
            {

                return;
            }
            
            UIbar.GetComponent<RectTransform>().anchoredPosition = WorldToUgui(barPoint.position);

            if (timeLeft <= 0 && !alwaysVisible)
            {
                UIbar.gameObject.SetActive(false);
            }
            else
            {
                timeLeft -= Time.deltaTime;
            }
        }
        public  Vector2 WorldToUgui(Vector3 position)
        {

            Vector2 screenPoint = Camera.main.WorldToScreenPoint(position);//世界坐标转换为屏幕坐标
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            screenPoint -= screenSize / 2;//将屏幕坐标变换为以屏幕中心为原点
            Vector2 anchorPos = screenPoint / screenSize * canvas.GetComponent<RectTransform>().sizeDelta;//缩放得到UGUI坐标
            return anchorPos;
        }
    }
}
