package com.company.pr9.entity

import io.jmix.core.entity.annotation.JmixGeneratedValue
import io.jmix.core.metamodel.annotation.JmixEntity
import java.util.*
import javax.persistence.*

@JmixEntity
@Table(name = "ROUTE")
@Entity
open class Route {
    @JmixGeneratedValue
    @Column(name = "ID", nullable = false)
    @Id
    private val id: UUID? = null

    @Column(name = "DEPARTURE_POINT", nullable = false)
    private val departurePoint: String? = null

    @Column(name = "ARRIVAL_POINT", nullable = false)
    private val arrivalPoint: String? = null

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "STATION_ID")
    private val station: Station? = null

    @ManyToMany(mappedBy = "routes")
    private val buses: List<Bus> = ArrayList() // Constructors, getters, setters
}
