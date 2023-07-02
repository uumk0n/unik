package com.company.pr9.screen.station;

import io.jmix.ui.screen.*;
import com.company.pr9.entity.Station;

@UiController("tr_Station.edit")
@UiDescriptor("station-edit.xml")
@EditedEntityContainer("stationDc")
public class StationEdit extends StandardEditor<Station> {
}