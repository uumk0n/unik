package com.company.pr9.screen.station;

import io.jmix.ui.screen.*;
import com.company.pr9.entity.Station;

@UiController("tr_Station.browse")
@UiDescriptor("station-browse.xml")
@LookupComponent("stationsTable")
public class StationBrowse extends StandardLookup<Station> {
}