﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using VapLib;

namespace CustomJobs
{
    public class ImportUserDB
    {
        static void Main(string[] args)
        {
            VapEntities db = new VapEntities();

            Console.WriteLine("导入用户数据");
            string fileName = args[0];
            DataTable content = OpenCSV(fileName);

            bool Step1 = false;
            bool Step2 = false;
            bool Step3 = false;
            bool Step4 = false;

            bool Step5 = false;
            bool Step6 = false;
            bool Step7 = false;

            bool Step8 = false;

            bool Step9 = true;

            //Import users;     //7181, 4579, 81025
            if (Step1)
            {
                Console.WriteLine("====================== Step 1 ======================");
                foreach (DataRow dr in content.Rows)
                {
                    var userName = dr["网号"].ToString();
                    var level = dr["级别"].ToString();
                    var achievement = decimal.Parse(dr["兑换券"].ToString());
                    var points = achievement * 0.15m;
                    var chongXiao = decimal.Parse(dr["重复消费"].ToString());
                    var coin1 = decimal.Parse(dr["溢出联合积分"].ToString());
                    var coin2 = decimal.Parse(dr["联合积分"].ToString());
                    var cash1 = decimal.Parse(dr["电子货币"].ToString());
                    var cash11 = decimal.Parse(dr["可报单货币额"].ToString());
                    var password1 = "123456";
                    var password2 = "654321";
                    var realName = dr["姓名"].ToString();
                    var phone = dr["电话"].ToString();
                    var email = dr["Email"].ToString();
                    var referral = dr["推荐用户"].ToString();
                    var s = dr["公司确认"].ToString();

                    var date = DateTime.Now;
                    var dStr = dr["确认时间"].ToString();
                    if (dStr != null && !"NULL".Equals(dStr, StringComparison.InvariantCultureIgnoreCase))
                    {
                        date = DateTime.Parse(dStr);
                    }
                    var status = s.Equals("Y", StringComparison.InvariantCultureIgnoreCase) ? 会员状态.正常.ToString() : 会员状态.待审核.ToString();

                    db.Members.Add(new Member
                    {
                        UserName = userName,
                        Email = userName + "@lianhe319.com",
                        RealName = realName,
                        Mobile = phone,
                        Password1 = password1,
                        Password2 = password2,
                        Status = status,
                        Cash1 = cash1 + cash11,
                        Cash2 = 0,
                        Coin1 = coin1,
                        Coin2 = coin2,
                        ChongXiao1 = chongXiao,
                        ChongXiao2 = 0,
                        Point1 = points,
                        Point2 = 0,
                        Achievement = achievement,
                        RegisterTime = date,
                        MemberLevel_Id = GetMemberLevelId(db, level),
                    });
                    Console.WriteLine(userName + " - " + level + " Inserted");
                    db.SaveChanges();
                }
            }

            //Update referral 
            if (Step2)
            {
                Console.WriteLine("====================== Step 2 ======================");
                foreach (DataRow dr in content.Rows)
                {
                    var userName = dr["网号"].ToString();
                    var referral = dr["推荐用户"].ToString();

                    Member member = db.Members.SingleOrDefault(m => m.UserName == userName);
                    Member mReferral = db.Members.SingleOrDefault(m => m.UserName == referral);
                    if (member.Referral_Id == null && mReferral != null)
                    {
                        member.Referral_Id = mReferral.Id;
                        db.SaveChanges();
                        Console.WriteLine(userName + " - Referral updated");
                    }
                }
            }


            //Insert Baodan buy record
            //Insert lock coins record
            if (Step3)
            {
                Console.WriteLine("====================== Step 3 ======================");
                foreach (DataRow dr in content.Rows)
                {
                    var userName = dr["网号"].ToString();
                    var buyPrice = decimal.Parse(dr["购买价格"].ToString());
                    var lastPrice = decimal.Parse(dr["上次溢出价格"].ToString());
                    var coin1 = decimal.Parse(dr["溢出联合积分"].ToString());
                    var coin2 = decimal.Parse(dr["联合积分"].ToString());

                    if (coin1 > 0 || coin2 > 0)
                    {
                        Member member = db.Members.SingleOrDefault(m => m.UserName == userName);
                        if (member != null)
                        {
                            var date = DateTime.Now;
                            var dStr = dr["确认时间"].ToString();
                            if (dStr != null && !"NULL".Equals(dStr, StringComparison.InvariantCultureIgnoreCase))
                            {
                                date = DateTime.Parse(dStr);
                            }
                            BaoDanTransactions mBaoDan = new BaoDanTransactions
                            {
                                DateTime = date,
                                Amount = coin1 + coin2,
                                Price = buyPrice,
                                Fee = 0,
                                Status = 报单状态.已成交.ToString(),
                                Type = 报单类型.买入.ToString(),
                            };
                            member.BaoDanTransactions.Add(mBaoDan);

                            member.LockedCoins.Add(new LockedCoins
                            {
                                Price = buyPrice,
                                LastPrice = lastPrice,
                                NextPrice = Math.Ceiling(lastPrice * 1.05m * 1000) / 1000,
                                TotalAmount = coin1 + coin2,
                                LockedAmount = coin2,
                                AvailabeAmount = coin1,
                                BaoDanTransactions = mBaoDan,
                            });
                            db.SaveChanges();
                            Console.WriteLine(userName + " - BaoDan updated");
                        }
                    }
                }
            }
            
            //Update achievement
            if (Step4)
            {
                Console.WriteLine("====================== Step 4 ======================");
                foreach (DataRow dr in content.Rows)
                {
                    var userName = dr["网号"].ToString();
                    var achievement = decimal.Parse(dr["兑换券"].ToString());
                    Member member = db.Members.SingleOrDefault(m => m.UserName == userName);
                    if (member != null)
                    {
                        UpdateReferralAchievement(member, achievement);
                        Console.WriteLine(userName + " - Referral achievement updated");
                    }
                }
                db.SaveChanges();
            }

            //Update user mail address
            if (Step5)
            {
                Console.WriteLine("====================== Step 5 ======================");
                foreach (DataRow dr in content.Rows)
                {
                    var userName = dr["网号"].ToString();
                    Member member = db.Members.SingleOrDefault(m => m.UserName == userName);
                    if (member != null)
                    {
                        member.Email = userName + "@lianhe319.com";
                        Console.WriteLine(member.Email + " - Email updated");
                    }
                }
                db.SaveChanges();
            }

            //Update user cash1
            if (Step6)
            {
                Console.WriteLine("====================== Step 6 ======================");
                foreach (DataRow dr in content.Rows)
                {
                    var userName = dr["网号"].ToString();
                    var cash1 = decimal.Parse(dr["电子货币"].ToString());
                    var cash11 = decimal.Parse(dr["可报单货币额"].ToString());
                    Member member = db.Members.SingleOrDefault(m => m.Email == userName + "@lianhe319.com");
                    if (member != null)
                    {
                        member.Cash1 = cash1 + cash11;
                        Console.WriteLine(member.Email + " - Cash updated");
                    }
                }
                db.SaveChanges();
            }

            //Update user register date
            if (Step7)
            {
                Console.WriteLine("====================== Step 7 ======================");
                foreach (DataRow dr in content.Rows)
                {
                    var userName = dr["网号"].ToString();
                    var dStr = dr["确认时间"].ToString();
                    if(dStr != null && !"NULL".Equals(dStr, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var date = DateTime.Parse(dStr);
                        Member member = db.Members.SingleOrDefault(m => m.Email == userName + "@lianhe319.com");
                        if (member != null)
                        {
                            member.RegisterTime = date;
                            foreach (var b in member.BaoDanTransactions)
                            {
                                b.DateTime = date;
                            }
                            Console.WriteLine(member.Email + " - Register Date updated");
                            db.SaveChanges();
                        }
                    }
                }
            }

            if (Step8)
            {
                Console.WriteLine("====================== Step 8 ======================");
                string priceFileName = args[1];
                DataTable priceContent = OpenCSV(priceFileName);
                foreach (DataRow dr in priceContent.Rows)
                {
                    decimal price = decimal.Parse(dr["价格"].ToString());
                    DateTime date = DateTime.Parse(dr["日期"].ToString());
                    var coin = new CoinPrices
                    {
                        Price = price,
                        DateTime = date,
                    };
                    db.CoinPrices.Add(coin);
                    db.SaveChanges();
                }
            }

            if (Step9)
            {
                Console.WriteLine("====================== Step 9 ======================");
                foreach (DataRow dr in content.Rows)
                {
                    var userName = dr["网号"].ToString();
                    var buyPrice = decimal.Parse(dr["购买价格"].ToString());
                    var lastPrice = decimal.Parse(dr["上次溢出价格"].ToString());
                    var coin1 = decimal.Parse(dr["溢出联合积分"].ToString());
                    var coin2 = decimal.Parse(dr["联合积分"].ToString());

                    if (coin1 > 0 || coin2 > 0)
                    {
                        Member member = db.Members.SingleOrDefault(m => m.UserName == userName);
                        if (member != null)
                        {
                            var mBaoDan = member.BaoDanTransactions.SingleOrDefault();
                            if (mBaoDan != null)
                            {
                                mBaoDan.Price = buyPrice;
                            }

                            var mLockedCoin = member.LockedCoins.SingleOrDefault();
                            if (mLockedCoin != null)
                            {
                                mLockedCoin.Price = buyPrice;
                                mLockedCoin.LastPrice = lastPrice;
                                mLockedCoin.NextPrice = Math.Ceiling(lastPrice * 1.05m * 1000) / 1000;
                            }

                            db.SaveChanges();
                            Console.WriteLine(userName + " - BaoDan Price updated");
                        }
                    }
                }
            }

            if (db != null)
            {
                db.Dispose();
                db = null;
            }
        }

        private static void UpdateReferralAchievement(Member member, decimal amount)
        {
            Member referral = member.Referral;
            if(referral !=null )
            {
                referral.Achievement += amount;
                UpdateReferralAchievement(referral, amount);
            }
        }


        private static int GetMemberLevelId(VapEntities db, string value)
        {
            value = value.Replace("1", "一");
            value = value.Replace("2", "二");
            value = value.Replace("3", "三");
            value = value.Replace("4", "四");
            value = value.Replace("5", "五");
            value = value.Replace("6", "六");
            value = value.Replace("7", "七");
            var v = db.MemberLevel.SingleOrDefault(m => m.Level == value);
            if(v != null)
            {
                return v.Id;
            }
            else
            {
                throw new Exception("Can't find member levle");
            }
        }

        private static DataTable OpenCSV(string filePath)
        {
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.Default);

            string strLine = "";
            string[] aryLine = null;
            string[] tableHead = null;
            int columnCount = 0;
            bool IsFirst = true;
            while ((strLine = sr.ReadLine()) != null)
            {
                if (IsFirst == true)
                {
                    tableHead = strLine.Split(',');
                    IsFirst = false;
                    columnCount = tableHead.Length;
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(tableHead[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    aryLine = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j].ToString();
                    }
                    dt.Rows.Add(dr);
                }
            }
            if (aryLine != null && aryLine.Length > 0)
            {
                dt.DefaultView.Sort = tableHead[0] + " " + "asc";
            }

            sr.Close();
            fs.Close();
            return dt;
        }
    }


}
