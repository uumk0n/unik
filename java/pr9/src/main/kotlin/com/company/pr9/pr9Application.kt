package com.company.pr9

import org.slf4j.LoggerFactory
import org.springframework.beans.factory.annotation.Autowired
import org.springframework.boot.SpringApplication
import org.springframework.boot.autoconfigure.SpringBootApplication
import org.springframework.boot.autoconfigure.jdbc.DataSourceProperties
import org.springframework.boot.context.event.ApplicationStartedEvent
import org.springframework.boot.context.properties.ConfigurationProperties
import org.springframework.context.annotation.Bean
import org.springframework.context.annotation.Primary
import org.springframework.context.event.EventListener
import org.springframework.core.env.Environment
import javax.sql.DataSource

@SpringBootApplication
open class pr9Application {

    companion object {
        @JvmStatic
        fun main(args: Array<String>) {
            SpringApplication.run(Lab7Application::class.java, *args)
        }
    }

    @Autowired
    private val environment: Environment? = null

    @Bean
    @Primary
    @ConfigurationProperties("main.datasource")
    open fun dataSourceProperties(): DataSourceProperties = DataSourceProperties()

    @Bean
    @Primary
    @ConfigurationProperties("main.datasource.hikari")
    open fun dataSource(dataSourceProperties: DataSourceProperties): DataSource =
        dataSourceProperties.initializeDataSourceBuilder().build()

    @EventListener
    open fun printApplicationUrl(event: ApplicationStartedEvent?) {
        LoggerFactory.getLogger(Lab7Application::class.java).info(
        "Application started at http://localhost:"
        + (environment?.getProperty("local.server.port") ?: "")
        + (environment?.getProperty("server.servlet.context-path") ?: ""))
    }
}