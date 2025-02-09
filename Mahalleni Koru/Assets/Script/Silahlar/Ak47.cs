using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ak47 : MonoBehaviour
{
    Animator animatorum;


    [Header("Ayarlar")]
    public bool atesEdebilirmi;
    float iceridenAtesEtmeSikligi;
    public float disaridanAtesEtmeSikligi;
    public float menzil;
    public GameObject cross;


    [Header("Sesler")]
    public AudioSource AtesSesi;
    public AudioSource SarjorSesi;
    public AudioSource MermiBitmeSesi;
    public AudioSource MermiAlmaSesi;


    [Header("Efektler")]
    public ParticleSystem AtesEfekti;
    public ParticleSystem MermiIzi;
    public ParticleSystem KanEfekti;

    [Header("Di�erleri")]
    public Camera benimCam;
    float camFieldPov;
    public float yaklasmaPov;

    [Header("Silah Ayarlar�")]
    int ToplamMermi;
    public int SarjorKapasite;
    int KalanMermi;
    public string silahin_Adi;
    public TextMeshProUGUI toplamMermi_Text;
    public TextMeshProUGUI KalanMermi_Text;
    public GameObject kovanCikis_noktasi;
    public GameObject kovanObjesi;
    public GameObject mermi_Cikis_Noktasi;
    public GameObject mermi;
    public float darbeGucu;

    public bool kovanCiksinMi;
    bool zoomVarmi;


    void Start()
    {
        ToplamMermi = PlayerPrefs.GetInt(silahin_Adi + "_Mermi");
        kovanCiksinMi = true;
        Baslangic_Mermi_Doldurma();
        sarjorDoldurmaTeknik("NormalYaz");
        animatorum = GetComponent<Animator>();
        camFieldPov = benimCam.fieldOfView;
    }

    void Update()
    {
        kovanCiksinMi = true;
        if (Input.GetKey(KeyCode.Mouse0) && !Input.GetKey(KeyCode.Mouse1))
        {
            if (atesEdebilirmi && Time.time > iceridenAtesEtmeSikligi && KalanMermi != 0)
            {
                if (!GameKontrolcu.oyunDurdurmu)
                {
                    AtesEt(false);
                    iceridenAtesEtmeSikligi = Time.time + disaridanAtesEtmeSikligi;
                }

            }
            if (KalanMermi == 0)
            {
                MermiBitmeSesi.Play();
            }


        }
        if (Input.GetKey(KeyCode.R))
        {
            if (KalanMermi < SarjorKapasite)
            {
                animatorum.Play("sarjorDegis");
            }

        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            mermiAl();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            zoomVarmi = true;
            animatorum.SetBool("zoom", true);
            cross.SetActive(false);

        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            zoomVarmi = false;
            animatorum.SetBool("zoom", false);
            benimCam.fieldOfView = camFieldPov;
            cross.SetActive(true);

        }

        if (zoomVarmi)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (atesEdebilirmi && Time.time > iceridenAtesEtmeSikligi && KalanMermi != 0)
                {
                    AtesEt(true);
                    iceridenAtesEtmeSikligi = Time.time + disaridanAtesEtmeSikligi;
                }
                if (KalanMermi == 0)
                {
                    MermiBitmeSesi.Play();
                }
            }
        }
    }

    void KameraYaklastir()
    {
        benimCam.fieldOfView = yaklasmaPov;
    }
    void mermiAl()
    {
        RaycastHit hit;
        if (Physics.Raycast(benimCam.transform.position, benimCam.transform.forward, out hit, 4))
        {
            if (hit.transform.gameObject.CompareTag("Mermi"))
            {
                mermiKaydet(hit.transform.gameObject.GetComponent<mermiKutusu>().Olusan_silahin_Turu, hit.transform.gameObject.GetComponent<mermiKutusu>().Olusan_mermi_sayisi);
                Mermi_Kutusu_Olustur.mermi_Kutusu_Varmi = false;
                Destroy(hit.transform.parent.gameObject);
            }
        }
    }
    void Baslangic_Mermi_Doldurma()
    {
        if (ToplamMermi <= SarjorKapasite)
        {
            KalanMermi = ToplamMermi;
            ToplamMermi = 0;
            PlayerPrefs.SetInt(silahin_Adi + "_Mermi", 0);
        }
        else
        {
            KalanMermi = SarjorKapasite;
            ToplamMermi -= SarjorKapasite;
            PlayerPrefs.SetInt(silahin_Adi + "_Mermi", ToplamMermi);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Mermi"))
        {
            mermiKaydet(other.gameObject.GetComponent<mermiKutusu>().Olusan_silahin_Turu, other.gameObject.GetComponent<mermiKutusu>().Olusan_mermi_sayisi);
            Mermi_Kutusu_Olustur.mermi_Kutusu_Varmi = false;
            Destroy(other.transform.parent.gameObject);

        }
        if (other.gameObject.CompareTag("Saglik"))
        {
            FindObjectOfType<GameKontrolcu>().Saglik_Al();
            Saglik_Kutusu_Olustur.saglik_Kutusu_Varmi = false;
            Destroy(other.transform.gameObject);

        }
        if (other.gameObject.CompareTag("Bomba_Kutusu"))
        {
            FindObjectOfType<GameKontrolcu>().Bomba_Al();
            Bomba_Kutusu_Olustur.bomba_Kutusu_Varmi = false;
            Destroy(other.transform.gameObject);
        }
    }
    IEnumerator CameraTitre(float titremeSuresi, float magnitude)
    {
        Vector3 orjinalPozisyon = benimCam.transform.localPosition;

        float gecenSure = 0.0f;
        while (gecenSure < titremeSuresi)
        {
            float x = Random.Range(-1, 1) * magnitude;

            benimCam.transform.localPosition = new Vector3(x, orjinalPozisyon.y, orjinalPozisyon.z);
            gecenSure += Time.deltaTime;
            yield return null;
        }
        benimCam.transform.localPosition = orjinalPozisyon;
    }
    void mermiKaydet(string silahTuru, int mermiSayisi)
    {
        MermiAlmaSesi.Play();
        switch (silahTuru)
        {
            case "Keles":
                ToplamMermi += mermiSayisi;
                PlayerPrefs.SetInt(silahin_Adi + "_Mermi", ToplamMermi);
                sarjorDoldurmaTeknik("NormalYaz");
                break;

            case "Magnum":
                PlayerPrefs.SetInt("Magnum_Mermi", PlayerPrefs.GetInt("Magnum_Mermi") + mermiSayisi);
                break;

            case "Pompali":
                PlayerPrefs.SetInt("Pompali_Mermi", PlayerPrefs.GetInt("Pompali_Mermi") + mermiSayisi);
                break;

            case "Sniper":
                PlayerPrefs.SetInt("Sniper_Mermi", PlayerPrefs.GetInt("Sniper_Mermi") + mermiSayisi);
                break;
        }
    }
    void SarjorSes()
    {
        SarjorSesi.Play();
        if (KalanMermi < SarjorKapasite)
        {
            if (KalanMermi != 0 && ToplamMermi != 0)
            {
                sarjorDoldurmaTeknik("MermiVar");
            }
            else if (ToplamMermi == 0)
            {
                sarjorDoldurmaTeknik("MermiYok");
            }
            else
            {
                sarjorDoldurmaTeknik("MermiYok");
            }

        }
    }
    void sarjorDoldurmaTeknik(string tur)
    {
        switch (tur)
        {
            case "MermiVar":
                if (ToplamMermi <= SarjorKapasite)
                {
                    int distaKalanMermi = ToplamMermi + KalanMermi;
                    if (distaKalanMermi > SarjorKapasite)
                    {
                        KalanMermi = SarjorKapasite;
                        ToplamMermi = distaKalanMermi - SarjorKapasite;
                        PlayerPrefs.SetInt(silahin_Adi + "_Mermi", ToplamMermi);

                    }
                    else
                    {
                        KalanMermi += ToplamMermi;
                        ToplamMermi = 0;
                        PlayerPrefs.SetInt(silahin_Adi + "_Mermi", 0);

                    }
                }
                else
                {
                    ToplamMermi -= SarjorKapasite - KalanMermi;
                    KalanMermi = SarjorKapasite;
                    PlayerPrefs.SetInt(silahin_Adi + "_Mermi", ToplamMermi);
                }

                toplamMermi_Text.text = ToplamMermi.ToString();
                KalanMermi_Text.text = KalanMermi.ToString();
                break;

            case "MermiYok":
                if (ToplamMermi <= SarjorKapasite)
                {
                    KalanMermi = ToplamMermi;
                    ToplamMermi = 0;
                    PlayerPrefs.SetInt(silahin_Adi + "_Mermi", 0);

                }
                else
                {
                    ToplamMermi -= SarjorKapasite;
                    PlayerPrefs.SetInt(silahin_Adi + "_Mermi", ToplamMermi);
                    KalanMermi = SarjorKapasite;

                }


                toplamMermi_Text.text = ToplamMermi.ToString();
                KalanMermi_Text.text = KalanMermi.ToString();
                break;

            case "NormalYaz":
                toplamMermi_Text.text = ToplamMermi.ToString();
                KalanMermi_Text.text = KalanMermi.ToString();
                break;
        }
    }
    void AtesEt(bool yakinlastirmaVarmi)
    {
        atesEtmeTeknikleri(yakinlastirmaVarmi);
        RaycastHit hit;
        if (Physics.Raycast(benimCam.transform.position, benimCam.transform.forward, out hit, menzil))
        {
            if (hit.transform.gameObject.CompareTag("Dusman"))
            {
                Instantiate(KanEfekti, hit.point, Quaternion.LookRotation(hit.normal));
                hit.transform.gameObject.GetComponent<Dusman>().DarbeAl(darbeGucu);
            }
            else if (hit.transform.gameObject.CompareTag("DevrilebilirObje"))
            {
                Rigidbody rg = hit.transform.gameObject.GetComponent<Rigidbody>();
                rg.AddForce(-hit.normal * 50f);
                Instantiate(MermiIzi, hit.point, Quaternion.LookRotation(hit.normal));
            }
            else
            {
                Instantiate(MermiIzi, hit.point, Quaternion.LookRotation(hit.normal));
            }

        }

    }

    void atesEtmeTeknikleri(bool yakinlastirmaVarmi)
    {
        if (kovanCiksinMi)
        {
            GameObject obje = Instantiate(kovanObjesi, kovanCikis_noktasi.transform.position, kovanCikis_noktasi.transform.rotation);
            Rigidbody rb = obje.GetComponent<Rigidbody>();
            rb.AddRelativeForce(new Vector3(-10, 1, 0) * 60);
        }

        Instantiate(mermi, mermi_Cikis_Noktasi.transform.position, mermi_Cikis_Noktasi.transform.rotation);
        StartCoroutine(CameraTitre(.1f, .04f));
        AtesSesi.Play();
        AtesEfekti.Play();
        if (!yakinlastirmaVarmi)
        {
            animatorum.Play("atesEt");
        }
        else
        {
            animatorum.Play("zoomAtesEt");
        }
        KalanMermi--;
        KalanMermi_Text.text = KalanMermi.ToString();
    }

}
