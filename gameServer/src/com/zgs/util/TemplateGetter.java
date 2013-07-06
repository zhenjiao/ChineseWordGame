package com.zgs.util;

import org.springframework.beans.factory.BeanFactory;
import org.springframework.beans.factory.xml.XmlBeanFactory;
import org.springframework.core.io.ClassPathResource;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.transaction.PlatformTransactionManager;
import org.springframework.transaction.support.TransactionTemplate;

public class TemplateGetter {

	//static private JdbcTemplate jtl;
	//static private TransactionTemplate ttl;
	private static ClassPathResource resource ;
	private static BeanFactory factory;
	
	static public JdbcTemplate getJtl() {
		if (resource==null)	resource = new ClassPathResource("applicationContext.xml");
		if (factory==null) factory= new XmlBeanFactory(resource);
		return (JdbcTemplate)factory.getBean("jdbcTemplate");
	}
	
	static public TransactionTemplate getttl() {
		if (resource==null)	resource = new ClassPathResource("applicationContext.xml");
		if (factory==null) factory= new XmlBeanFactory(resource);
		return (TransactionTemplate)new TransactionTemplate((PlatformTransactionManager) factory.getBean("transactionManager"));
	}
}
