package com.company.pr9.entity

import io.jmix.core.entity.annotation.JmixGeneratedValue
import io.jmix.core.metamodel.annotation.InstanceName
import io.jmix.core.metamodel.annotation.JmixEntity
import java.util.*
import javax.persistence.*

@JmixEntity
@Table(name = "BUS")
@Entity
open class Bus {
    @JmixGeneratedValue
    @Column(name = "ID", nullable = false)
    @Id
    private var internalId: String? = null

    fun getId(): String? {
        return internalId
    }

    fun setId(id: String?) {
        this.internalId = id
    }
    @InstanceName
    @Column(name = "MODEL")
    private val model: String? = null

    @Column(name = "REGISTRATION_NUMBER")
    private val registrationNumber: String? = null

    @Column(name = "SEATING_CAPACITY")
    private val seatingCapacity = 0

    @Temporal(TemporalType.DATE)
    @Column(name = "START_DATE")
    private val startDate: Date? = null

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "STATION_ID")
    private val station: Station? = null

    @ManyToMany
    @JoinTable(
        name = "BUS_ROUTE_LINK",
        joinColumns = [JoinColumn(name = "BUS_ID")],
        inverseJoinColumns = [JoinColumn(name = "ROUTE_ID")]
    )
    private val routes: List<Route> = ArrayList<Route>()

    // Constructors, getters, setters
}
