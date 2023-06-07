package com.company.pr9.entity

import io.jmix.core.entity.annotation.JmixGeneratedValue
import io.jmix.core.metamodel.annotation.InstanceName
import io.jmix.core.metamodel.annotation.JmixEntity
import java.util.*
import javax.persistence.*

@JmixEntity
@Table(name = "STATION")
@Entity
open class Station {
    @JmixGeneratedValue
    @Column(name = "ID", nullable = false)
    @Id
    private val id: UUID? = null

    @InstanceName
    @Column(name = "NAME")
    private val name: String? = null

    @OneToMany(mappedBy = "station")
    private val routes: List<Route> = ArrayList<Route>()

    @OneToMany(mappedBy = "station")
    private val buses: List<Bus> = ArrayList() // Constructors, getters, setters
}
