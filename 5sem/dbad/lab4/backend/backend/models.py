from django.db import models

class HFInfo(models.Model):
    Inila = models.CharField(primary_key=True, max_length=255)
    Status = models.CharField(max_length=255)
    IdInfo = models.CharField(max_length=255, null=True, blank=True)
    DateOrd = models.DateTimeField(null=True, blank=True)
    RegOrgNum = models.CharField(max_length=255, db_index=True)
    class Meta:
        db_table = 'HfInfos'

class EduDocs(models.Model):
    Inila = models.CharField(primary_key=True, max_length=255)
    SndEduDoc = models.CharField(max_length=255)
    DateEnd = models.DateTimeField(null=True, blank=True)
    RegEduDoc = models.CharField(max_length=255, db_index=True)
    class Meta:
        db_table = 'EduDocs'

class Institut(models.Model):
    RegEduDoc = models.CharField(primary_key=True, max_length=255)
    Type = models.CharField(max_length=255)
    Name = models.CharField(max_length=255)
    class Meta:
        db_table = 'Instituts'

class PersonalData(models.Model):
    Fcs = models.CharField(primary_key=True, max_length=255)
    Itn = models.CharField(max_length=255)
    Address = models.CharField(max_length=255)
    SnPassport = models.CharField(max_length=255)
    Married = models.BooleanField(null=True, blank=True)
    Kids = models.IntegerField(null=True, blank=True)
    Inila = models.ForeignKey('HFInfo', on_delete=models.CASCADE, db_column='Inila')

    class Meta:
        db_table = 'PersonalDatas'


class Work(models.Model):
    RegOrgNum = models.CharField(primary_key=True, max_length=255)
    NameOrg = models.CharField(max_length=255)
    ItnOrg = models.CharField(max_length=255)
    OrgAddress = models.CharField(max_length=255)
    class Meta:
        db_table = 'Works'
