﻿using Common.Services;
using Common.Tools.Database;
using Common.Tools.WebSite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    [TestClass]
    public class NotificationServiceTest
    {
        string[] inputData = { };

        [TestMethod]
        //有効な通知先情報が1つの場合
        public void DoServiceTest01()
        {
            //Arrange
            List<Account> accounts = new List<Account>
            {
                new Account
                {
                    is_valid = 1,
                    id = "testId",
                    password = "testPwd",
                    access_token = "oA4C4FdNKL9ZB9Uo90XwUJ05vr15Cw2yNA2bhUrbh4h"
                }
            };

            EventInfo eventInfo = new EventInfo
            {
                date = "2020.03.29",
                title = "testTitle",
                artist = "testArtist"
            };

            int expectedListSize = accounts.Count;
            string expectedToken = accounts[0].access_token;
            string expectedMsg = Messages.AM02 + Messages.URL;


            IDbAccessor db = new StubDbAccessor(accounts);
            IEventInfoConverter ei = new StubEventInfoConverter(eventInfo);
            MockLineMessenger lm = new MockLineMessenger();

            //Act
            using (NotificationService service = new NotificationService(db, ei, lm))
            {
                service.DoService(inputData);
            }

            //Assert
            int actualListSize = lm.InputList.Count;
            string actualToken = lm.InputList[0].accessToken;
            string actualMsg = lm.InputList[0].message;

            Assert.IsTrue(expectedListSize == actualListSize);
            Assert.AreEqual(expectedToken, actualToken);
            Assert.AreEqual(expectedMsg, actualMsg);

        }

        [TestMethod]
        //有効な通知先情報が2つの場合
        public void DoServiceTest02()
        {
            //Arrange
            List<Account> accounts = new List<Account>
            {
                new Account
                {
                    is_valid = 1,
                    id = "testId0",
                    password = "testPwd0",
                    access_token = "testToken0"
                },
                new Account
                {
                    is_valid = 1,
                    id = "testId1",
                    password = "testPwd1",
                    access_token = "testToken1"
                }
            };

            EventInfo eventInfo = new EventInfo
            {
                date = "2020.03.29",
                title = "testTitle",
                artist = "testArtist"
            };

            int expectedListSize = accounts.Count;
            string[] expectedToken = { accounts[0].access_token, accounts[1].access_token };

            string expectedMsg = Messages.AM02 + Messages.URL;

            IDbAccessor db = new StubDbAccessor(accounts);
            IEventInfoConverter ei = new StubEventInfoConverter(eventInfo);
            MockLineMessenger lm = new MockLineMessenger();

            //Act
            using (NotificationService service = new NotificationService(db, ei, lm))
            {
                service.DoService(inputData);
            }

            //Assert
            int actualListSize = lm.InputList.Count;
            string[] actualToken = { lm.InputList[0].accessToken, lm.InputList[1].accessToken };
            string[] actualMsg = { lm.InputList[0].message, lm.InputList[1].message };

            Assert.IsTrue(actualListSize == expectedListSize);
            for (int i = 0; i < actualListSize; i++)
            {
                Assert.AreEqual(expectedToken[i], actualToken[i]);
                Assert.AreEqual(expectedMsg, actualMsg[i]);
            }
        }

        [TestMethod]
        //有効な通知先情報がない場合
        public void DoServiceTest03()
        {
            //Arrange
            List<Account> accounts = new List<Account>();

            EventInfo eventInfo = new EventInfo
            {
                date = "2020.03.29",
                title = "testTitle",
                artist = "testArtist"
            };

            int expectedListSize = accounts.Count;

            IDbAccessor db = new StubDbAccessor(accounts);
            IEventInfoConverter ei = new StubEventInfoConverter(eventInfo);
            MockLineMessenger lm = new MockLineMessenger();

            //Act
            using (NotificationService service = new NotificationService(db, ei, lm))
            {
                service.DoService(inputData);
            }

            //Assert
            int actualListSize = lm.InputList.Count;

            Assert.IsTrue(expectedListSize == actualListSize);
        }


        [TestMethod]
        //有効な通知先情報が100個の場合
        public void DoServiceTest04()
        {
            //Arrange
            int n = 100;

            List<Account> accounts = new List<Account>();

            for (int i = 0; i < n; i++)
            {
                accounts.Add(new Account
                {
                    is_valid = 1,
                    id = "testId0",
                    password = "testPwd0",
                    access_token = "oA4C4FdNKL9ZB9Uo90XwUJ05vr15Cw2yNA2bhUrbh4" + i.ToString()
                }); ;
            }


            EventInfo eventInfo = new EventInfo
            {
                date = "2020.03.29",
                title = "testTitle",
                artist = "testArtist"
            };

            int expectedListSize = accounts.Count;

            string expectedMsg = Messages.AM02 + Messages.URL;

            IDbAccessor db = new StubDbAccessor(accounts);
            IEventInfoConverter ei = new StubEventInfoConverter(eventInfo);
            MockLineMessenger lm = new MockLineMessenger();

            //Act
            using (NotificationService service = new NotificationService(db, ei, lm))
            {
                service.DoService(inputData);
            }

            //Assert
            int actualListSize = lm.InputList.Count;

            Assert.IsTrue(actualListSize == expectedListSize);
            for (int i = 0; i < actualListSize; i++)
            {
                Assert.AreEqual(accounts[i].access_token, lm.InputList[i].accessToken);
                Assert.AreEqual(expectedMsg, lm.InputList[i].message);
            }
        }
        [TestMethod]
        //有効な通知先情報=10, うち送信失敗=3
        public void DoServiceTest05()
        {
            //Arrange
            int n = 10;

            List<Account> accounts = new List<Account>();

            for (int i = 0; i < n; i++)
            {
                accounts.Add(new Account
                {
                    is_valid = 1,
                    id = "testId0",
                    password = "testPwd0",
                    access_token = i % 3 == 1 ? "false" : "oA4C4FdNKL9ZB9Uo90XwUJ05vr15Cw2yNA2bhUrbh4"
                }); ;
            }


            EventInfo eventInfo = new EventInfo
            {
                date = "2020.03.29",
                title = "testTitle",
                artist = "testArtist"
            };

            var expectedList = accounts.Where(x => x.access_token != "false").ToList();
            int expectedListSize = expectedList.Count;

            string expectedMsg = Messages.AM02 + Messages.URL;

            IDbAccessor db = new StubDbAccessor(accounts);
            IEventInfoConverter ei = new StubEventInfoConverter(eventInfo);
            MockLineMessenger lm = new MockLineMessenger();

            //Act
            using (NotificationService service = new NotificationService(db, ei, lm))
            {
                service.DoService(inputData);
            }

            //Assert
            int actualListSize = lm.InputList.Count;

            Assert.IsTrue(actualListSize == expectedListSize);
            for (int i = 0; i < actualListSize; i++)
            {
                Assert.AreEqual(expectedList[i].access_token, lm.InputList[i].accessToken);
                Assert.AreEqual(expectedMsg, lm.InputList[i].message);
            }
        }

        [TestMethod]
        //イベント情報を取得できない場合
        public void DoServiceTest10()
        {
            //Arrange
            List<Account> accounts = new List<Account>
            {
                new Account
                {
                    is_valid = 1,
                    id = "testId",
                    password = "testPwd",
                    access_token = "oA4C4FdNKL9ZB9Uo90XwUJ05vr15Cw2yNA2bhUrbh4h"
                }
            };

            EventInfo eventInfo = new EventInfo
            {
                date = "---",
                title = "testTitle",
                artist = "testArtist"
            };

            int expectedListSize = accounts.Count;
            string expectedToken = accounts[0].access_token;
            string expectedMsg = Messages.AM03 + Messages.URL;


            IDbAccessor db = new StubDbAccessor(accounts);
            IEventInfoConverter ei = new StubEventInfoConverter(eventInfo);
            MockLineMessenger lm = new MockLineMessenger();

            //Act
            using (NotificationService service = new NotificationService(db, ei, lm))
            {
                service.DoService(inputData);
            }

            //Assert
            int actualListSize = lm.InputList.Count;
            string actualToken = lm.InputList[0].accessToken;
            string actualMsg = lm.InputList[0].message;

            Assert.IsTrue(expectedListSize == actualListSize);
            Assert.AreEqual(expectedToken, actualToken);
            Assert.AreEqual(expectedMsg, actualMsg);

        }

        [TestMethod]
        //イベントが本日でない場合
        public void DoServiceTest11()
        {
            //Arrange
            List<Account> accounts = new List<Account>
            {
                new Account
                {
                    is_valid = 1,
                    id = "testId",
                    password = "testPwd",
                    access_token = "oA4C4FdNKL9ZB9Uo90XwUJ05vr15Cw2yNA2bhUrbh4h"
                }
            };

            EventInfo eventInfo = new EventInfo
            {
                date = "2020.03.20",
                title = "testTitle",
                artist = "testArtist"
            };

            int expectedListSize = accounts.Count;
            string expectedToken = accounts[0].access_token;
            string expectedMsg = Messages.AM02 + Messages.URL;


            IDbAccessor db = new StubDbAccessor(accounts);
            IEventInfoConverter ei = new StubEventInfoConverter(eventInfo);
            MockLineMessenger lm = new MockLineMessenger();

            //Act
            using (NotificationService service = new NotificationService(db, ei, lm))
            {
                service.DoService(inputData);
            }

            //Assert
            int actualListSize = lm.InputList.Count;
            string actualToken = lm.InputList[0].accessToken;
            string actualMsg = lm.InputList[0].message;

            Assert.IsTrue(expectedListSize == actualListSize);
            Assert.AreEqual(expectedToken, actualToken);
            Assert.AreEqual(expectedMsg, actualMsg);

        }

        [TestMethod]
        //イベントが本日の場合、タイトルとアーティストあり
        public void DoServiceTest12()
        {
            //Arrange
            List<Account> accounts = new List<Account>
            {
                new Account
                {
                    is_valid = 1,
                    id = "testId",
                    password = "testPwd",
                    access_token = "oA4C4FdNKL9ZB9Uo90XwUJ05vr15Cw2yNA2bhUrbh4h"
                }
            };

            EventInfo eventInfo = new EventInfo
            {
                date = "2020.03.30",
                title = "testTitle",
                artist = "testArtist"
            };

            int expectedListSize = accounts.Count;
            string expectedToken = accounts[0].access_token;
            string expectedMsg = Messages.AM01("testTitle", "testArtist") + Messages.URL;


            IDbAccessor db = new StubDbAccessor(accounts);
            IEventInfoConverter ei = new StubEventInfoConverter(eventInfo);
            MockLineMessenger lm = new MockLineMessenger();

            //Act
            using (NotificationService service = new NotificationService(db, ei, lm))
            {
                service.DoService(inputData);
            }

            //Assert
            int actualListSize = lm.InputList.Count;
            string actualToken = lm.InputList[0].accessToken;
            string actualMsg = lm.InputList[0].message;

            Assert.IsTrue(expectedListSize == actualListSize);
            Assert.AreEqual(expectedToken, actualToken);
            Assert.AreEqual(expectedMsg, actualMsg);

        }

        [TestMethod]
        //イベントが本日の場合、タイトルなし、アーティストあり
        public void DoServiceTest13()
        {
            //Arrange
            List<Account> accounts = new List<Account>
            {
                new Account
                {
                    is_valid = 1,
                    id = "testId",
                    password = "testPwd",
                    access_token = "oA4C4FdNKL9ZB9Uo90XwUJ05vr15Cw2yNA2bhUrbh4h"
                }
            };

            EventInfo eventInfo = new EventInfo
            {
                date = "2020.03.30",
                title = "",
                artist = "testArtist"
            };

            int expectedListSize = accounts.Count;
            string expectedToken = accounts[0].access_token;
            string expectedMsg = Messages.AM01("", "testArtist") + Messages.URL;


            IDbAccessor db = new StubDbAccessor(accounts);
            IEventInfoConverter ei = new StubEventInfoConverter(eventInfo);
            MockLineMessenger lm = new MockLineMessenger();

            //Act
            using (NotificationService service = new NotificationService(db, ei, lm))
            {
                service.DoService(inputData);
            }

            //Assert
            int actualListSize = lm.InputList.Count;
            string actualToken = lm.InputList[0].accessToken;
            string actualMsg = lm.InputList[0].message;

            Assert.IsTrue(expectedListSize == actualListSize);
            Assert.AreEqual(expectedToken, actualToken);
            Assert.AreEqual(expectedMsg, actualMsg);

        }

        [TestMethod]
        //イベントが本日の場合、タイトルあり、アーティストなし
        public void DoServiceTest14()
        {
            //Arrange
            List<Account> accounts = new List<Account>
            {
                new Account
                {
                    is_valid = 1,
                    id = "testId",
                    password = "testPwd",
                    access_token = "oA4C4FdNKL9ZB9Uo90XwUJ05vr15Cw2yNA2bhUrbh4h"
                }
            };

            EventInfo eventInfo = new EventInfo
            {
                date = "2020.03.30",
                title = "testTitle",
                artist = ""
            };

            int expectedListSize = accounts.Count;
            string expectedToken = accounts[0].access_token;
            string expectedMsg = Messages.AM01("testTitle", "") + Messages.URL;


            IDbAccessor db = new StubDbAccessor(accounts);
            IEventInfoConverter ei = new StubEventInfoConverter(eventInfo);
            MockLineMessenger lm = new MockLineMessenger();

            //Act
            using (NotificationService service = new NotificationService(db, ei, lm))
            {
                service.DoService(inputData);
            }

            //Assert
            int actualListSize = lm.InputList.Count;
            string actualToken = lm.InputList[0].accessToken;
            string actualMsg = lm.InputList[0].message;

            Assert.IsTrue(expectedListSize == actualListSize);
            Assert.AreEqual(expectedToken, actualToken);
            Assert.AreEqual(expectedMsg, actualMsg);

        }

    }
}
