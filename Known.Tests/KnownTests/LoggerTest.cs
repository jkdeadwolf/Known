﻿using System;
using System.IO;
using Known.Log;

namespace Known.Tests.KnownTests
{
    public class LoggerTest
    {
        public static void TestConsoleLogger()
        {
            var logger = new ConsoleLogger();
            for (int i = 0; i < 3; i++)
            {
                logger.Trace("测试Trace{0}", i);
            }
            logger.Info("测试结束");
            logger.Info("这是Trace信息：{0}", logger.TraceInfo);
            logger.Error("发生未知错误");
        }

        public static void TestFileLogger()
        {
            var fileName = string.Format("{0}\\test\\test.log", Environment.CurrentDirectory);
            Utils.DeleteFile(fileName);

            var logger = new FileLogger(fileName);
            for (int i = 0; i < 3; i++)
            {
                logger.Trace("测试Trace{0}", i);
            }
            logger.Info("测试结束");
            logger.Info("这是Trace信息：{0}", logger.TraceInfo);
            logger.Error("发生未知错误");

            var log = File.ReadAllText(fileName);
            Assert.IsNotNull(log);
        }
    }
}
