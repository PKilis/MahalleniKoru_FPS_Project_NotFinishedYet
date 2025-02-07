using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class GameKontrolcu : MonoBehaviour
{
    float health = 100;

    [Header("Silah Ayarlar�")]
    public GameObject[] silahlar;
    public AudioSource silahDegisimSesi;
    public GameObject bomba;
    public GameObject bombaPoint;
    public Camera benimCam;

    [Header("Dusman Ayarlar�")]
    public GameObject[] dusmanlar;
    public GameObject[] cikisNoktalar;
    public GameObject[] hedefNoktalar;
    public TextMeshProUGUI kalanDusman_Text;
    public int baslangic_Dusman_Sayisi;
    public int dusmanCikmaSuresi;

    [Header("Sa�l�k Ayarlar�")]
    public Image healthBar;

    [Header("Di�er Ayarlar�")]
    public GameObject gameOverCanvas;
    public GameObject kazandinCanvas;
    public GameObject pauseCanvas;
    public TextMeshProUGUI saglik_Sayisi_Text;
    public TextMeshProUGUI bomba_Sayisi_Text;
    public AudioSource itemYok;



    public static int kalan_Dusman_Sayisi;
    public static bool oyunDurdurmu;


    void Start()
    {

        BaslangicIslemleri();

    }

    void Update()
    {
        if (!oyunDurdurmu)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SilahDegistir(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SilahDegistir(1);
            }
            else if (Input.GetKeyDown(KeyCode.G))
            {
                BombaAt();
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                if (healthBar.fillAmount < 1)
                {
                    SaglikDoldur();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                Pause();
            }
        }


    }

    IEnumerator DusmanCikar()
    {
        while (true)
        {
            yield return new WaitForSeconds(dusmanCikmaSuresi);

            if (baslangic_Dusman_Sayisi != 0)
            {
                int dusman = Random.Range(0, 4);
                int cikisYeri = Random.Range(0, 2);
                int hedefNoktasi = Random.Range(0, 2);

                GameObject obje = Instantiate(dusmanlar[dusman], cikisNoktalar[cikisYeri].transform.position, Quaternion.identity);
                obje.GetComponent<Dusman>().HedefBelirle(hedefNoktalar[hedefNoktasi]);
                baslangic_Dusman_Sayisi--;
            }

        }
    }

    void BaslangicIslemleri()
    {
        oyunDurdurmu = false;

        if (!PlayerPrefs.HasKey("OyunBasladiMi"))
        {
            PlayerPrefs.SetInt("Taramali_Mermi", 250);
            PlayerPrefs.SetInt("Pompali_Mermi", 50);
            PlayerPrefs.SetInt("Magnum_Mermi", 80);
            PlayerPrefs.SetInt("Sniper_Mermi", 20);
            PlayerPrefs.SetInt("Saglik_Sayisi", 3);
            PlayerPrefs.SetInt("Bomba_Sayisi", 2);

            PlayerPrefs.SetInt("OyunBasladiMi", 1);
        }
        kalanDusman_Text.text = baslangic_Dusman_Sayisi.ToString();
        kalanDusman_Text.text = baslangic_Dusman_Sayisi.ToString();


        saglik_Sayisi_Text.text = PlayerPrefs.GetInt("Saglik_Sayisi").ToString();
        bomba_Sayisi_Text.text = PlayerPrefs.GetInt("Bomba_Sayisi").ToString();

        kalan_Dusman_Sayisi = baslangic_Dusman_Sayisi;


        StartCoroutine(DusmanCikar());
    }
    public void Dusman_Sayisi_Guncelle()
    {
        kalan_Dusman_Sayisi--;
        kalanDusman_Text.text = kalan_Dusman_Sayisi.ToString();
        if (kalan_Dusman_Sayisi <= 0)
        {
            kalanDusman_Text.text = "0";
            kazandinCanvas.SetActive(true);
            Time.timeScale = 0;
        }
    }
    public void DarbeAl(float darbeGucu)
    {
        health -= darbeGucu;
        healthBar.fillAmount = health / 100;

        if (health <= 0)
        {
            GameOver();
        }
    }
    public void SaglikDoldur()
    {

        if (PlayerPrefs.GetInt("Saglik_Sayisi") != 0 && health <= 100)
        {
            health = 100;
            healthBar.fillAmount = health / 100;
            PlayerPrefs.SetInt("Saglik_Sayisi", PlayerPrefs.GetInt("Saglik_Sayisi") - 1);
            saglik_Sayisi_Text.text = PlayerPrefs.GetInt("Saglik_Sayisi").ToString();

        }
        else
        {
            itemYok.Play();
        }
    }
    public void Saglik_Al()
    {
        PlayerPrefs.SetInt("Saglik_Sayisi", PlayerPrefs.GetInt("Saglik_Sayisi") + 1);
        saglik_Sayisi_Text.text = PlayerPrefs.GetInt("Saglik_Sayisi").ToString();
    }
    public void Bomba_Al()
    {
        PlayerPrefs.SetInt("Bomba_Sayisi", PlayerPrefs.GetInt("Bomba_Sayisi") + 1);
        bomba_Sayisi_Text.text = PlayerPrefs.GetInt("Bomba_Sayisi").ToString();

    }
    private void BombaAt()
    {
        if (PlayerPrefs.GetInt("Bomba_Sayisi") != 0)
        {
            GameObject objem = Instantiate(bomba, bombaPoint.transform.position, bombaPoint.transform.rotation);
            Rigidbody rb = objem.GetComponent<Rigidbody>();
            Vector3 acimiz = Quaternion.AngleAxis(90, benimCam.transform.forward) * benimCam.transform.forward;
            rb.AddForce(acimiz * 250);

            PlayerPrefs.SetInt("Bomba_Sayisi", PlayerPrefs.GetInt("Bomba_Sayisi") - 1);
            bomba_Sayisi_Text.text = PlayerPrefs.GetInt("Bomba_Sayisi").ToString();
        }
        else
        {
            itemYok.Play();
        }

    }

    private void GameOver()
    {
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0;
        oyunDurdurmu = true;
        Cursor.visible = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>().m_MouseLook.lockCursor = false;
        Cursor.lockState = CursorLockMode.None;


    }

    void SilahDegistir(int siraNumarasi)
    {
        foreach (GameObject silah in silahlar)
        {
            silah.SetActive(false);
        }
        silahDegisimSesi.Play();
        silahlar[siraNumarasi].SetActive(true);
    }
    public void BastanBasla()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
        oyunDurdurmu = false;
        Cursor.visible = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>().m_MouseLook.lockCursor = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void AnaMenu()
    {
        SceneManager.LoadScene(0);

    }
    public void Pause()
    {
        pauseCanvas.SetActive(true);
        Time.timeScale = 0;
        oyunDurdurmu = true;
        Cursor.visible = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>().m_MouseLook.lockCursor = false;
        Cursor.lockState = CursorLockMode.None;
    }
    public void DevamEt()
    {
        pauseCanvas.SetActive(false);
        Time.timeScale = 1;
        oyunDurdurmu = false; 
        Cursor.visible = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>().m_MouseLook.lockCursor = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
}