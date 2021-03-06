﻿/*
*Author : Don
*Since 	:2020-04-18
*Description  : NHibernate帮助类
*/
using NHibernate;
using NHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;
using FluentNHibernate;
using FluentNHibernate.Automapping;

namespace AscensionServer
{
    public class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;

        public static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    _sessionFactory = Fluently.Configure().
                        Database(MySQLConfiguration.Standard.
                          //ConnectionString(db => db.Server("127.0.0.1").Database("cricket").Username("jieyou").Password("jieyougamePWD"))).//公网
                          ConnectionString(db => db.Server("192.168.0.117").Database("cricket").Username("jieyou").Password("jieyougamePWD"))).//内网
                          //ConnectionString(db => db.Server("121.196.189.220").Database("cricket").Username("root").Password("yingduan"))).//内网
                        Mappings(x => { x.FluentMappings.AddFromAssemblyOf<NHibernateHelper>(); }).
                        BuildSessionFactory();
                }
                return _sessionFactory;
            }
        }

        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();//打开一个跟数据库的会话
        }
    }

}
