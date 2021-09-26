<template>
  <div>
    <div class="container-fluid mt--5">
      <div class="row">
        <div class="col-xl-12">
          <card header-classes="bg-transparent" shadow shadowSize="3">
            <template v-slot:header>
              <div class="row">
                <div class="col-md-6">
                  <h1>Användare</h1>
                </div>
              </div>
            </template>
            <div class="row">
              <div class="col-md-12">
                <table class="hoverTable">
                  <tr>
                    <td>
                      <p style="float:left;margin-top:-40px;font-size:12px;color:#cdcdcd">Admin</p>
                      <label
                        class="checkdiv"
                        style="vertical-align: top; margin-top: -12px"
                      >                      
                        <input                          
                          type="checkbox"
                          v-model="user.isAdmin"
                          title="Administratör"
                        />
                        <span class="checkmark"></span>
                      </label>
                    </td>
                    <td>
                      <input
                        class="form-control form-control-alternative"
                        v-model="user.name"
                        placeholder="Namn"
                      />
                    </td>
                    <td>
                      <input
                        class="form-control form-control-alternative"
                        v-model="user.userName"
                        placeholder="Användarnamn"
                      />
                    </td>
                    <td>
                      <input
                        class="form-control form-control-alternative"
                        v-model="user.password"
                        placeholder="Lösenord"
                      />
                    </td>
                    <td>
                      <button class="btn btn-secondary" v-on:click="addUser">
                        Lägg till
                      </button>
                    </td>
                    <td></td>
                  </tr>
                  <tr v-for="user in users" :key="user.userName">
                    <td>
                      <label
                        class="checkdiv"
                        style="vertical-align: top; margin-top: -12px"
                      >
                        <input
                          type="checkbox"
                          v-model="user.isAdmin"
                          v-on:click="changeIsAdmin(user)"
                          title="Administratör"
                        />
                        <span class="checkmark"></span>
                      </label>
                    </td>
                    <td>
                      <input
                        type="text"
                        class="form-control form-control-alternative"
                        v-model="user.name"
                        placeholder="Namn"
                      />
                    </td>
                    <td>
                      <input
                        type="text"
                        class="form-control form-control-alternative"
                        v-model="user.userName"
                        placeholder="Inloggning"
                      />
                    </td>
                    <td>
                      <input
                        type="text"
                        class="form-control form-control-alternative"
                        v-model="user.password"
                        placeholder="Ange nytt lösenord"
                      />
                    </td>
                    <td>
                      <button
                        class="btn btn-secondary"
                        v-on:click="saveUser(user)">Spara</button>
                    </td>
                    <td>
                      <i class="far fa-trash-alt" style="cursor:pointer" title="Ta bort" v-on:click="deleteUser(user.id)"></i>
                    </td>
                  </tr>
                </table>
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
  name: "UserAdmin",
  data: function () {
    return {
      users: [],
      user: {
        id: null,
        name: null,
        userName: null,
        password: null,
        isAdmin: false,
      },
    };
  },
  mounted: function () {
    this.loadUsers();
  },
  methods: {
    addUser() {
      console.log(this.user);
      axios
        .post("/api/admin/adduser", this.user)
        .catch((e) => alert(e))
        .then(() => this.loadUsers());
    },
    deleteUser(id) {
      if (confirm("Vill du verkligen ta bort denna användaren?")) {
        var body = {
          id: id
        };
        axios
          .post("/api/admin/delete", body)
          .catch((e) => alert(e))
          .then(() => this.loadUsers());
      }
    },
    loadUsers() {
      fetch("/api/admin/users")
        .then((response) => response.json())
        .then((users) => {
          this.users = users;
        });
    },
    saveUser(user) {
      axios.post("/api/admin/update", user).then(() => {
        alert("Användaren sparad");
      });
    },
    changeIsAdmin(user) {
      var body = {
        userId: user.id,
        isAdmin: !user.isAdmin,
      };
      
      let currentUser = JSON.parse(localStorage.getItem("currentUser"));
      if (!body.isAdmin && currentUser.name == user.name) {
        alert('Not allowed to remove admin permission from yourself!');
        user.isAdmin = true;
        return;
      }
    },
  },
};
</script>
