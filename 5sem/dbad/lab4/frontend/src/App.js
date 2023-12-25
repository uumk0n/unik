// App.js
import React from 'react';
import TabbedTables from './TabbedTables';

const tabs = [
  { label: 'EduDocs', apiUrl: 'http://localhost:8000/edu-docs/', columns: ['Inila', 'RegEduDoc', 'SndEduDoc', 'DateEnd'], model: 'EduDoc' },
  { label: 'HfInfos', apiUrl: 'http://localhost:8000/hf-infos/', columns: ['IdInfo', 'Inila', 'RegOrgNum', 'Status', 'DateOrd'], model: 'HFInfo' },
  { label: 'Institutes', apiUrl: 'http://localhost:8000/instituts/', columns: ['RegEduDoc', 'Type', 'Name'], model: 'Institut' },
  { label: 'PersonalData', apiUrl: 'http://localhost:8000/personal-datas/', columns: ['Inila', 'Fcs', 'Itn', 'Address', 'SnPassport', 'Married', 'Kids'], model: 'PersonalData' },
  { label: 'Works', apiUrl: 'http://localhost:8000/works/', columns: ['RegOrgNum', 'NameOrg', 'ItnOrg', 'OrgAddress'], model: 'Work' },
];

function App() {
  return (
    <div className="App">
      <TabbedTables tabs={tabs} />
    </div>
  );
}

export default App;
