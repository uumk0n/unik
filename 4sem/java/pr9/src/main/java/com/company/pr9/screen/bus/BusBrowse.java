package com.company.pr9.screen.bus;

import io.jmix.ui.screen.*;
import com.company.pr9.entity.Bus;

@UiController("tr_Bus.browse")
@UiDescriptor("bus-browse.xml")
@LookupComponent("busesTable")
public class BusBrowse extends StandardLookup<Bus> {
}