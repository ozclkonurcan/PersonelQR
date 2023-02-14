using Barkod.MVC.Models;
using Newtonsoft.Json;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Barkod.MVC.Controllers
{
    public class PersonelController : Controller
    {

        //LOGİN CONTROLLER
        public ActionResult personelKayitLogin(YetkiliPersonel yetkiliPersonel)
        {
            try
            {

                NameValueCollection section =
       (NameValueCollection)ConfigurationManager.GetSection("MyDictionary");

               
               

           
              
                List<YetkiliPersonel> personelGiris = new List<YetkiliPersonel>()
            {
                new YetkiliPersonel(){id=1,k_ad="corumgaz",k_sifre="crm_20220609",k_durum="Yetkili"},
                new YetkiliPersonel(){id=2,k_ad="gyitmez",k_sifre="17C797Eg",k_durum="Yetkili"},
                new YetkiliPersonel(){id=3,k_ad="IKbetul",k_sifre="123654bg",k_durum="Yetkili"},
                new YetkiliPersonel(){id=4,k_ad="halparslan",k_sifre="598604",k_durum="Yetkili"},
                //new YetkiliPersonel(){id=4,k_ad="personelQRadmin",k_sifre="159456.a0",k_durum="Admin"},
                //new YetkiliPersonel(){id=3,k_ad="deneme2",k_sifre="c",k_durum="Personel2"}
            };


                var den = section[yetkiliPersonel.k_ad];
                if (den != null && den == yetkiliPersonel.k_sifre)
            {
                   

                  

                    var resp = (from a in personelGiris select a).ToList();

                var giris = resp.Any(x => x.k_ad.Trim() == yetkiliPersonel.k_ad.Trim() && x.k_sifre.Trim() == yetkiliPersonel.k_sifre.Trim());

                if (giris)
                {
                    var respData = (from a in personelGiris where a.k_ad.Trim() == yetkiliPersonel.k_ad.Trim() && a.k_sifre.Trim() == yetkiliPersonel.k_sifre.Trim() select a).FirstOrDefault();

                    Session["K_ID"] = respData.id;
                    Session["kullaniciAdi"] = respData.k_ad;
                    Session["kullaniciDurum"] = respData.k_durum;
                

                    TempData["SuccessMessage"] = "Giriş Başarılı";
                    return RedirectToAction("personelEkle", "personel");
                }
                else
                {
                    TempData["ErrorMessage"] = "Giriş Başarısız";
                    return RedirectToAction("");
                }

                }
                else
                {
                    
                    return View();
                }


            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Beklenmeyen bir hata meydana geldi tekrar giriş yapmayı deneyin \n" + ex.Message;
                return RedirectToAction("");
            }
        }

        public ActionResult logout(YetkiliPersonel yetkiliPersonel)
        {
            try
            {

            int K_ID = (int)Session["K_ID"];
            Session.Abandon();
            TempData["InfoMessage"] = "Çıkış başarılı";
            return RedirectToAction("personelKayitLogin", "personel");

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Beklenmeyen bir hata meydana geldi tekrar giriş yapmayı deneyin \n"+ ex.Message;
                return RedirectToAction("");
            }
        }


        //LOGİN CONTROLLER



        // GET: Personel
        public ActionResult Index(string tc, string allSearch, string unvanSearch, string sirketSearch, string bolumSearch, string isyeriSearch)
         {
            if (Session["kullaniciDurum"] != null)
            {

            try
            {

                string filePath = Server.MapPath("~/JsonData/personel.json");
                List<AhlPersonel> res = JsonConvert.DeserializeObject<List<AhlPersonel>>(System.IO.File.ReadAllText(filePath));


            //    HttpResponseMessage response = _env.WebApiClient.GetAsync("personel").Result;
            //HttpContent content = response.Content;
            //string jsonstring = content.ReadAsStringAsync().Result;



            //var res = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AhlPersonel>>(jsonstring);
            var personelList = (from a in res select a).ToList();
            var personelListBolum = (from a in res select a.bolum).Distinct().ToList();
            var personelListIsyeri = (from a in res select a.isyeri).Distinct().ToList();
            var personelListUnvan = (from a in res select a.unvan).Distinct().ToList();
            var personelListSirket = (from a in res select a.sirket).Distinct().ToList();

            
            if (!string.IsNullOrEmpty(allSearch))
            {
                personelList = personelList.Where(x => x.ad_soyad.ToLower().Contains(allSearch.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(unvanSearch))
            {
                personelList = personelList.Where(x => x.unvan.ToLower() == unvanSearch.ToLower()).ToList();
            }

            if (!string.IsNullOrEmpty(sirketSearch))
            {
                personelList = personelList.Where(x => x.sirket.ToLower() == sirketSearch.ToLower()).ToList();
            }
            if (!string.IsNullOrEmpty(bolumSearch))
            {
                personelList = personelList.Where(x => x.bolum.ToLower() == bolumSearch.ToLower()).ToList();
            }
            if (!string.IsNullOrEmpty(isyeriSearch))
            {
                personelList = personelList.Where(x => x.isyeri.ToLower() == isyeriSearch.ToLower()).ToList();
            }


            if (tc != null)
            {
                var personelList2 = (from a in res.Where(x => x.tc == tc) select a).FirstOrDefault();

                QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
                QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode("[TC : '" + personelList2.tc + "'] / " +
                    "[AD SOYAD : '" + personelList2.ad_soyad.ToUpper() + "'] / " +
                    "[ÜNVAN : '" + personelList2.unvan.ToUpper() + "'] / " +
                    "[İŞ YERİ : '" + personelList2.isyeri.ToUpper() + "'] / " +
                    "[BÖLÜM : '" + personelList2.bolum.ToUpper() + "'] / " +
                    "[ŞİRKET : '" + personelList2.sirket.ToUpper() + "']", QRCodeGenerator.ECCLevel.Q);
                QRCode qRCode = new QRCode(qRCodeData);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (Bitmap bitmap = qRCode.GetGraphic(20))
                    {
                        bitmap.Save(ms, ImageFormat.Png);
                        ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                        ViewBag.PerList = personelList2;
                    }
                }
            }

            

            ViewBag.personelBolum = personelListBolum;
            ViewBag.personelSirket = personelListSirket;
            ViewBag.personelListIsyeri = personelListIsyeri;
            ViewBag.personelListUnvan = personelListUnvan;
            ViewBag.personelListBag = personelList;


                    return RedirectToAction("personelEkle", "personel");
            //return View(personelList);

            }
            catch (Exception)
            {

                return RedirectToAction("personelEkle","personel");
            }

            }
            else
            {
                TempData["InfoMessage"] = "Giriş yapmadan sayfalar arası geçiş yapamazsın !!";
                return RedirectToAction("personelKayitLogin", "personel");
            }
        }


        //POST: Personel Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult personelPost(AhlPersonel ahlPersonel)
        {
            if (Session["kullaniciDurum"] != null)
            {

                try
            {
                string filePath = Server.MapPath("~/JsonData/personel.json");
                List<AhlPersonel> res = JsonConvert.DeserializeObject<List<AhlPersonel>>(System.IO.File.ReadAllText(filePath));

                //HttpResponseMessage response2 = _env.WebApiClient.GetAsync("personel").Result;
                //HttpContent content = response2.Content;
                //string jsonstring = content.ReadAsStringAsync().Result;
                //var res = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AhlPersonel>>(jsonstring);
                var personelList = (from a in res select a).ToList();


            


                if (ahlPersonel.id == 0)
            {

                    if (personelList.Any(x => x.tc == ahlPersonel.tc))
                    {
                        TempData["WarnMessage"] = "Daha önce bu kimlik numarasına sahip bir personel oluşturulmuş.";
                        return RedirectToAction("");
                    }
                    else
                    {
                    
                    HttpResponseMessage response = _env.WebApiClient.PostAsJsonAsync("personel", ahlPersonel).Result;
                        
                    if (response.IsSuccessStatusCode)
                    {
                    TempData["SuccessMessage"] = "Personel Oluşturuldu...";
                    }
                    else
                    {
                        TempData["WarnMessage"] = "Personel Oluşturulamadı !!!";
                    }
                        return RedirectToAction("");
                            }
                }
            else
            {

                   

                    HttpResponseMessage response = _env.WebApiClient.PutAsJsonAsync("personel/" + ahlPersonel.id, ahlPersonel).Result;
                    if (response.IsSuccessStatusCode)
                    {
                    TempData["InfoMessage"] = "Personel Güncellendi...";
                    }
                    else
                    {
                        TempData["WarnMessage"] = "Personel Güncelleştirilemedi !!!";
                    }
                    return RedirectToAction("");
            }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "HATA\r\n"+ex.Message;
                return RedirectToAction("");
            }


        }
            else
            {
                TempData["InfoMessage"] = "Giriş yapmadan sayfalar arası geçiş yapamazsın !!";
                return RedirectToAction("personelKayitLogin", "personel");
    }
}


        public JsonResult updateRespData(int id)
        {
            

            try
            {

                if(id != 0) {
                    string filePath = Server.MapPath("~/JsonData/personel.json");
                    List<AhlPersonel> res = JsonConvert.DeserializeObject<List<AhlPersonel>>(System.IO.File.ReadAllText(filePath));

                    var personelList = (from a in res where a.id == id select a).FirstOrDefault();


                    var text = "TC : '" + personelList.tc + "'\r\n" +
                       "AD SOYAD : '" + personelList.ad_soyad.ToUpper() + "'\r\n " +
                       "ÜNVAN : '" + personelList.unvan.ToUpper() + "'\r\n " +
                       "İŞ YERİ : '" + personelList.isyeri.ToUpper() + "'\r\n " +
                       "BÖLÜM : '" + personelList.bolum.ToUpper() + "'\r\n " +
                       "ŞİRKET : '" + personelList.sirket.ToUpper() + "'\r\n";

                    QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
                    QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                    QRCode qRCode = new QRCode(qRCodeData);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (Bitmap bitmap = qRCode.GetGraphic(20))
                        {
                            bitmap.Save(ms, ImageFormat.Png);
                            ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                            ViewBag.PerList = personelList;

                            personelList.QRCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                        }
                    }


                    return Json(personelList);
                }
                else
                {
                    return Json(null);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "HATA\r\n" + ex.Message;
                return Json("");
            }

        }
        //POST: Personel Post


        //Personel Remove Control
        public ActionResult deletePersonel(int id)
        {
            if (Session["kullaniciDurum"] != null)
            {
           
            try
            {
            HttpResponseMessage response = _env.WebApiClient.DeleteAsync("personel/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
            TempData["SuccessMessage"] = "Silme işlemi başarılı...";
            return RedirectToAction("");
            }
            else
            {
                TempData["WarnMessage"] = "Silme işlemi gerçekleşemedi !!!";
                return RedirectToAction("");
            }


            }
            catch (Exception ex)
            {

                TempData["ErrorMessage"] = "HATA\r\n"+ex.Message;
                return RedirectToAction("");
            }
            }
            else
            {
                TempData["InfoMessage"] = "Giriş yapmadan sayfalar arası geçiş yapamazsın !!";
                return RedirectToAction("personelKayitLogin", "personel");
            }
        }
        //Personel Remove Control

        //GET: Personel Detail
        public ActionResult personelDetail(int id)
        {
            if (Session["kullaniciDurum"] != null)
            {
         
            try
            {
                string filePath = Server.MapPath("~/JsonData/personel.json");
                List<AhlPersonel> res = JsonConvert.DeserializeObject<List<AhlPersonel>>(System.IO.File.ReadAllText(filePath));


                var personelList = (from a in res where a.id == id select a).FirstOrDefault();
            
            if (id != null)
            {
                var text = "TC : '" + personelList.tc + "'\r\n" +
                        "AD SOYAD : '" + personelList.ad_soyad.ToUpper() + "'\r\n " +
                        "ÜNVAN : '" + personelList.unvan.ToUpper() + "'\r\n " +
                        "İŞ YERİ : '" + personelList.isyeri.ToUpper() + "'\r\n " +
                        "BÖLÜM : '" + personelList.bolum.ToUpper() + "'\r\n " +
                        "ŞİRKET : '" + personelList.sirket.ToUpper() + "'\r\n";

                QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
                QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                QRCode qRCode = new QRCode(qRCodeData);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (Bitmap bitmap = qRCode.GetGraphic(20))
                    {
                        bitmap.Save(ms, ImageFormat.Png);
                        ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                        ViewBag.PerList = personelList;
                    }
                }
            }
                    return RedirectToAction("personelEkle", "personel");
            //return View(personelList);

            }
            catch (Exception)
            {

                return RedirectToAction("");
            }
            }
            else
            {
                TempData["InfoMessage"] = "Giriş yapmadan sayfalar arası geçiş yapamazsın !!";
                return RedirectToAction("personelKayitLogin", "personel");
            }

        }


        //Post: Personel QR Print Page
        private static Byte[] BitmapToBytes(Bitmap img)
        {
           
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
        public ActionResult Print(string[] tc, string select_all,AhlPersonel[] ahlPersonel)
        {
            if (Session["kullaniciDurum"] != null)
            {
          
            try
            {

                if(ahlPersonel != null)
                {
                    tc = null;
                }
               



                if (tc != null && ahlPersonel == null) {
                    string filePath = Server.MapPath("~/JsonData/personel.json");
                    List<AhlPersonel> res = JsonConvert.DeserializeObject<List<AhlPersonel>>(System.IO.File.ReadAllText(filePath));


                    var personelList = (from a in res where tc.Contains(a.tc)  select a).ToList();

              

            if (tc != null && ahlPersonel == null)
            {

                foreach (var item in personelList)
                {

             
            
                    var text =  "TC : '" + item.tc + "'\r\n" +
                                "AD SOYAD : '" + item.ad_soyad.ToUpper() + "'\r\n " +
                                "ÜNVAN : '" + item.unvan.ToUpper() + "'\r\n " +
                                "İŞ YERİ : '" + item.isyeri.ToUpper() + "'\r\n " +
                                "BÖLÜM : '" + item.bolum.ToUpper() + "'\r\n " +
                                "ŞİRKET : '" + item.sirket.ToUpper() + "'\r\n";



                    QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
                    QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                    QRCode qRCode = new QRCode(qRCodeData);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (Bitmap bitmap = qRCode.GetGraphic(20))
                        {
                            bitmap.Save(ms, ImageFormat.Png);
                            item.QRCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                            ViewBag.PerList = personelList;
                        }
                    }
                }
            }
               
             
            

            return View(personelList);
            }
                else if (ahlPersonel != null && tc == null)
                {
                    if (ahlPersonel != null)
                    {
                        tc = null;




                        var personelList = (from a in ahlPersonel select a).ToList();

                        foreach (var item in personelList)
                        {
                            var text = "TC : '" + item.tc + "'\r\n" +
                                 "AD SOYAD : '" + item.ad_soyad.ToUpper() + "'\r\n " +
                                 "ÜNVAN : '" + item.unvan.ToUpper() + "'\r\n " +
                                 "İŞ YERİ : '" + item.isyeri.ToUpper() + "'\r\n " +
                                 "BÖLÜM : '" + item.bolum.ToUpper() + "'\r\n " +
                                 "ŞİRKET : '" + item.sirket.ToUpper() + "'\r\n";


                            QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
                            QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                            QRCode qRCode = new QRCode(qRCodeData);
                            using (MemoryStream ms = new MemoryStream())
                            {
                                using (Bitmap bitmap = qRCode.GetGraphic(20))
                                {
                                    bitmap.Save(ms, ImageFormat.Png);
                                    item.QRCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                                    ViewBag.PerList = personelList;
                                }
                            }
                        }







                        TempData["SuccessMessage"] ="QR Code Oluşturuldu.";
                        return View(ahlPersonel);
                    }
                    else
                    {
                        TempData["ErrorMessage"] ="QR Code Oluşturulamadı.";
                        return RedirectToAction("Index", "Personel");
                    }
                }
                else
            {
                return RedirectToAction("");
            }

            }
            catch (Exception)
            {

                return RedirectToAction("");
            }


            }
            else
            {
                TempData["InfoMessage"] = "Giriş yapmadan sayfalar arası geçiş yapamazsın !!";
                return RedirectToAction("personelKayitLogin", "personel");
            }
        }





        public ActionResult personelEkle()
        {
            if (Session["kullaniciDurum"] != null)
            {
            return View();
            }
            else
            {
                TempData["InfoMessage"] = "Giriş yapmadan sayfalar arası geçiş yapamazsın !!";

                return RedirectToAction("personelKayitLogin", "personel");
            }
        }


    }
}