<template>
  <div>
    <div class="container-fluid mt--5">
      <div class="row">
        <div class="col-xl-12">
          <card header-classes="bg-transparent" shadow shadowSize="3">
            <template v-slot:header>
              <div class="row">
                <div class="col-md-6">
                  <h1>Enkät</h1>
                  <h2>Carlsborgs Släktforskarförening</h2>
                </div>
                <div class="col-md-6 text-right">
                  <img src="img/carlsborglogo.png" height="100"/>
                </div>
              </div>
            </template>
            <div class="row">
              <table class="hoverTable forsvik-text">
                <tr>
                  <td width="200px">
                    <label>Namn</label>
                  </td>
                  <td>
                    <input
                      placeholder="Namn"
                      type="text"
                      class="form-control"
                      v-model="model.name"
                    />
                  </td>
                </tr>
                <tr>
                  <td width="200px">
                    <label>Email</label>
                  </td>
                  <td>
                    <input
                      placeholder="Email"
                      type="text"
                      class="form-control"
                      v-model="model.email"
                    />
                  </td>
                </tr>
                <tr>
                  <td><label>Vill du ha information via epost?</label></td>        
                  <td class="t300">
                    <div class="row">
                      <div class="col-3">
                        <label class="checkdiv">
                          <span style="font-size:16px">Ja</span>
                          <input type="checkbox" v-model="model.sendEmail" />
                          <span class="checkmark"></span>
                        </label>
                      </div>
                      <div class="col-3">
                        <label class="checkdiv">
                          <span style="font-size:16px">Nej</span>
                          <input type="checkbox" v-model="resetEmail" />
                          <span class="checkmark"></span>
                        </label>
                      </div>
                    </div>
                  </td>
                </tr>  
              </table>
              
              <table class="hoverTable forsvik-text">                                      
                <tr>
                  <td class="t300"><label>Hur vill du beteckna din släktforskning just nu?</label></td>        
                  <td></td>
                </tr>
                <tr>
                  <td><label>Aktiv</label></td>
                  <td>
                      <label class="checkdiv">
                        <input type="checkbox" v-model="model.stateRightNow1" />
                        <span class="checkmark"></span>
                      </label>
                  </td>
                </tr>  
                <tr>
                  <td><label>Halvfart</label></td>        
                  <td>
                      <div class="row"><div class="col-1">
                        <label class="checkdiv">
                          <input type="checkbox" v-model="model.stateRightNow2" />
                          <span class="checkmark"></span>
                        </label>
                      </div></div>
                  </td>
                </tr>  
                <tr>
                  <td><label>Vilande</label></td>        
                  <td>
                      <label class="checkdiv">
                        <input type="checkbox" v-model="model.stateRightNow3" />
                        <span class="checkmark"></span>
                      </label>
                  </td>
                </tr>                                                                        
              </table>
              <table class="hoverTable forsvik-text" style="margin-top:20px">
                <tr>
                  <td><label>Är du medlem i några andra släktforskarföreningar och i så fall vilka?</label></td>                          
                </tr>
                <tr>
                   <td>
                    <input
                      placeholder="Ange föreningar.."
                      type="text"
                      class="form-control"
                      v-model="model.otherAssociations"
                    />
                  </td>
                </tr>
              </table>
              <table class="hoverTable forsvik-text" style="margin-top:20px">
                <tr>
                  <td class="t300"><label>Använder du en dator i din släktforskning?</label></td>        
                  <td>
                    <div class="row">
                      <div class="col-2">
                        <label class="checkdiv">
                          <span style="font-size:16px">Ja</span>
                          <input type="checkbox" v-model="model.usingComputer" />
                          <span class="checkmark"></span>
                        </label>
                      </div>
                      <div class="col-3">
                        <label class="checkdiv">
                          <span style="font-size:16px">Nej</span>
                          <input type="checkbox" @click="model.usingComputer = false" />
                          <span class="checkmark"></span>
                        </label>
                      </div>
                    </div>
                  </td>
                </tr> 
                <tr>
                  <td class="t300"><label>Om du svarar ja, vilket släktforskningsprogram använder du?</label></td>  
                  <td></td>      
                </tr>      
                <tr>
                  <td><label>Disgen</label></td>        
                  <td>
                      <div class="row"><div class="col-1">
                        <label class="checkdiv">
                          <input type="checkbox" v-model="model.usingDisgen" />
                          <span class="checkmark"></span>
                        </label>
                      </div>
                      </div>                      
                  </td>
                </tr>    
                <tr>
                  <td><label>Min Släkt</label></td>        
                  <td>
                      <div class="row"><div class="col-1">
                        <label class="checkdiv">
                          <input type="checkbox" v-model="model.usingMinSlakt" />
                          <span class="checkmark"></span>
                        </label>
                      </div>
                      </div>                      
                  </td>
                </tr> 
                <tr>
                  <td><label>Annat</label></td>        
                  <td>
                      <div class="row">
                        <div class="col-1">
                        <label class="checkdiv">
                          <input type="checkbox" v-model="model.usingOther" />
                          <span class="checkmark"></span>
                        </label>
                      </div>
                      <div class="col-11">
                        <input
                          placeholder="Programnamn"
                          type="text"
                          class="form-control"
                          v-model="model.usingOtherProgramName"
                        />
                      </div>
                      </div>                      
                  </td>
                </tr>                                       
              </table>
              <table class="hoverTable forsvik-text" style="margin-top:20px">
                <tr>
                  <td class="t300"><label>Är stöddagar viktiga för dig?</label></td>        
                  <td>
                    <div class="row">
                      <div class="col-2">
                        <label class="checkdiv">
                          <span style="font-size:16px">Ja</span>
                          <input type="checkbox" v-model="model.interestedInProgram" />
                          <span class="checkmark"></span>
                        </label>
                      </div>
                      <div class="col-3">
                        <label class="checkdiv">
                          <span style="font-size:16px">Nej</span>
                          <input type="checkbox" @click="model.interestedInProgram = false" />
                          <span class="checkmark"></span>
                        </label>
                      </div>
                    </div>
                  </td>
                </tr> 
              </table>
              <table class="hoverTable forsvik-text" style="margin-top:20px">
                <tr>
                  <td class="t300"><label>Är du intresserad av kurser/utbildning inom släktforskning?</label></td>        
                  <td>
                    <div class="row">
                      <div class="col-2">
                        <label class="checkdiv">
                          <span style="font-size:16px">Ja</span>
                          <input type="checkbox" v-model="model.interestedInEducation" />
                          <span class="checkmark"></span>
                        </label>
                      </div>
                      <div class="col-3">
                        <label class="checkdiv">
                          <span style="font-size:16px">Nej</span>
                          <input type="checkbox" @click="model.interestedInEducation = false" />
                          <span class="checkmark"></span>
                        </label>
                      </div>
                    </div>
                  </td>
                </tr> 
                <tr>
                  <td>Om ja, vilken inriktning?</td>
                   <td>
                    <input
                      placeholder="Inriktining"
                      type="text"
                      class="form-control"
                      v-model="model.interestedInEducationText"
                    />
                  </td>
                </tr>
              </table>  
              <table class="hoverTable forsvik-text" style="margin-top:20px">
                <tr>
                  <td class="t300"><label>Använder du DNA-tekniken inom din forskning?</label></td>        
                  <td>
                    <div class="row">
                      <div class="col-2">
                        <label class="checkdiv">
                          <span style="font-size:16px">Ja</span>
                          <input type="checkbox" v-model="model.usesDna" />
                          <span class="checkmark"></span>
                        </label>
                      </div>
                      <div class="col-3">
                        <label class="checkdiv">
                          <span style="font-size:16px">Nej</span>
                          <input type="checkbox" @click="model.usesDna = false" />
                          <span class="checkmark"></span>
                        </label>
                      </div>
                    </div>
                  </td>
                </tr> 
                <tr>
                  <td>Om ja, vilket företag använder du?</td>
                   <td>
                    <input
                      placeholder="Inriktining"
                      type="text"
                      class="form-control"
                      v-model="model.usesDnaCompany"
                    />
                  </td>
                </tr>
              </table>                           
              <table class="hoverTable forsvik-text" style="margin-top:20px">
                <tr>
                  <td><label>Är det något du vill veta mer av?</label></td>                          
                </tr>
                <tr>
                   <td>
                    <input
                      placeholder="Dina egna tankar.."
                      type="text"
                      class="form-control"
                      v-model="model.knowMore"
                    />
                  </td>
                </tr>
              </table>           
              <table class="hoverTable forsvik-text" style="margin-top:20px">
                <tr>
                  <td><label>
                    Har du något att bidraga med för att hjälpa oss andra forskare?<br/>
                    Har du t.ex. använt protokoll av olika slag?
                  </label></td>                          
                </tr>
                <tr>
                   <td>
                    <input
                      placeholder="Dina ord.."
                      type="text"
                      class="form-control"
                      v-model="model.contribute"
                    />
                  </td>
                </tr>
              </table>           
              <table class="hoverTable forsvik-text" style="margin-top:20px">
                <tr>
                  <td><label>Styrelsen är tacksam för förslag på aktiviteter.</label></td>                          
                </tr>
                <tr>
                   <td>
                    <input
                      placeholder="Aktiviteter"
                      type="text"
                      class="form-control"
                      v-model="model.suggestions"
                    />
                  </td>
                </tr>
              </table>
              <table class="hoverTable forsvik-text" style="margin-top:20px">
                <tr>
                  <td><label>Det vore trevligt, om du vill skriv ner högst 5 socknar, där du har dina anfäder. Vi kanske är släkt!</label></td>                          
                </tr>
                <tr>
                   <td>
                    <textarea
                      placeholder="Socknar"
                      type="text"
                      class="form-control"
                      v-model="model.parish"
                    />
                  </td>
                </tr>
              </table>
              <table class="hoverTable forsvik-text" style="margin-top:20px">
                <tr>
                  <td class="t300"><label>Är Släktforskarföreningen viktig för dig?</label></td>        
                  <td>
                    <div class="row">
                      <div class="col-2">
                        <label class="checkdiv">
                          <span style="font-size:16px">Ja</span>
                          <input type="checkbox" v-model="model.isSlaktImportant" />
                          <span class="checkmark"></span>
                        </label>
                      </div>
                      <div class="col-3">
                        <label class="checkdiv">
                          <span style="font-size:16px">Nej</span>
                          <input type="checkbox" @click="model.isSlaktImportant = false" />
                          <span class="checkmark"></span>
                        </label>
                      </div>
                    </div>
                  </td>
                </tr> 
              </table>
            </div>
            <div class="row">
              <div class="col-md-2">
                <br/>
                <b>Tack för hjälpen!</b>
              </div>
              <div class="col-md-6">
                <button
                  type="button"
                  class="btn-forsvik"  
                  style="margin-top:20px"            
                  @click="sendPedigreeForm"              
                >Skicka formulär</button>
              </div>
            </div>

          </card>
        </div>
      </div>
    </div>
  </div>
</template>
<script>

import axios from "axios";

export default {
  data() {
    return {
      model: {
        name: '',
        address: '',
        phone: '',
        email: '',
        sendEmail: false,
      }
    };
  },
  methods: {
    resetEmail (check) {
      this.model.sendEmail = false;
      return check;
    },
    sendPedigreeForm() {
      axios.post("/api/pedigree/sendone", this.model)
        .then((response) => {
          if (response.data.result) {
            alert("Meddelandet skickat");
          } else {
            alert("Meddelandet kunde inte skickas: " + response.data.error);
          }
        });
    }
  }
};
</script>
<style scoped>
.t300 {
  width: 300px;
}
</style>
